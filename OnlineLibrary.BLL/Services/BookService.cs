using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.Common.Extensions;
using OnlineLibrary.BLL.Interfaces;
using OnlineLibrary.DAL.Interfaces;
using OnlineLibrary.Common.Exceptions;
using Microsoft.AspNetCore.JsonPatch;
using FluentValidation;
using OnlineLibrary.Common.EntityProcessing.Pagination;
using OnlineLibrary.Common.EntityProcessing;
using System.Threading.Tasks;
using System.Linq;
using FluentValidation.Results;
using System.Collections.Generic;
using System;
using OnlineLibrary.DAL.DTO;

namespace OnlineLibrary.BLL.Services
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IValidator<BookDTO> _bookDTOValidator;

        private readonly IValidator<Reservation> _reservationValidator;

        public BookService(IUnitOfWork uow, IValidator<BookDTO> bookValidator,IValidator<Reservation> reservationValidator)
        {
            _unitOfWork = uow;
            _bookDTOValidator = bookValidator;
            _reservationValidator = reservationValidator;
        }

        private int CalculateSkip(int count, int pageNumber, int pageSize)
        {
            // If we need to show more data than we have, show the first page.
            if (count < pageSize) return 0;

            // If after skipping data there is some data, show it.
            int skip = (pageNumber - 1) * pageSize;
            if (count - skip >= 1) return skip;

            // Else calculate how much we need to skip for obtaining the last page.
            int remainder = count % pageSize;
            if (remainder == 0) return count - pageSize;
            return count - remainder;
        }

        public async Task<PaginatedList<Book>> FilterSortPaginBooksAsync(BookProcessing bookProcessing)
        {
            List<int> filteredBookIds = await _unitOfWork.BookRepository.FilterBooksAsync(bookProcessing.Filtration);
            ExceptionExtensions.Check<OLNotFound>(!filteredBookIds.Any(), "Books not found");

            int skip = CalculateSkip(filteredBookIds.Count(), bookProcessing.Pagination.PageNumber, bookProcessing.Pagination.PageSize);
            bool fromBooks = false;
            if (!bookProcessing.Filtration.CheckFilterPropsNotNull())
            {
                fromBooks = true;
            }
            List<Book> paginatedBooksList = await _unitOfWork.BookRepository.SortPaginBooksAsync(filteredBookIds, fromBooks, bookProcessing.Sorting, skip, bookProcessing.Pagination.PageSize);

            ExceptionExtensions.Check<OLInternalServerError>(paginatedBooksList == null, "Can't load paginated books. Books not found.");
            
            return new PaginatedList<Book>(filteredBookIds.Count(), paginatedBooksList);
        }

        public async Task<Book> GetBookByIdAsync(int bookId)
        {
            Book book = await _unitOfWork.BookRepository.GetBookByIdAsync(bookId);
            ExceptionExtensions.Check<OLNotFound>(book == null, "Book not found");
            return book;
        }

        public async Task UpdatePatchAsync(int bookId, JsonPatchDocument<BookDTO> book)
        {
            ExceptionExtensions.Check<OLInternalServerError>(!book.Operations.All(x => x.op.ToUpper().Contains("REPLACE")), "this method supports only replace operations");

            var originalBook = new BookDTO(await _unitOfWork.BookRepository.GetBookByIdAsync(bookId));
            ExceptionExtensions.Check<OLNotFound>(originalBook == null, "Book not found");
            book.ApplyTo(originalBook);
            ExceptionExtensions.Check<OLBadRequest>(bookId != originalBook.Id, "You are trying to change the ID. it's not allowed.");

            ValidationResult bookResult = _bookDTOValidator.Validate(originalBook);
            ExceptionExtensions.Check<OLBadRequest>(!bookResult.IsValid, "The book has been changed incorrectly");

            // validate reservations 
            await CheckReservationValidation(bookId, originalBook.Reservations);
            
            // check if we can change archivation
            if (book.Operations.Any(x => x.path.ToUpper().Contains("INARCHIVE")))
            {
                ExceptionExtensions.Check<OLBadRequest>(originalBook.InArchive == true && originalBook.Reservations.Any(r => r.ReturnDate == null), "You can't archivate this book. It is in reserve.");
            }

            // validate AuthorBook if need
            if (book.Operations.Any(x => x.path.ToUpper().Contains("AUTHORS")))
            {
                originalBook.Authors = await _unitOfWork.AuthorRepository.GetAuthorsByIdListAsync(originalBook.Authors.Select(x => x.Id).ToList());
                ExceptionExtensions.Check<OLBadRequest>(!originalBook.Authors.Any(), "You are trying to change the Authors. But this authors don't exists.");
            }

            // validate BookTag if need
            if (book.Operations.Any(x => x.path.ToUpper().Contains("TAGS")))
            {
                originalBook.Tags = await _unitOfWork.TagRepository.GetTagsByIdListAsync(originalBook.Tags.Select(x => x.Id).ToList());
                ExceptionExtensions.Check<OLBadRequest>(!originalBook.Tags.Any(), "You are trying to change the Tags. But this tags don't exists.");
            }
            
            await _unitOfWork.BookRepository.UpdateBookAsync(originalBook);
        }

        private async Task CheckReservationValidation(int bookId, List<Reservation> reservations)
        {
            if (!reservations.Any()) return;

            ExceptionExtensions.Check<OLBadRequest>(!reservations.All(x => x.BookId == bookId), $"Not in all reservations bookId = {bookId}");

            int count = reservations.Where(r => r.ReturnDate == null).Count();
            ExceptionExtensions.Check<OLBadRequest>(count > 1, $"Only 1 person can read the book at a time. Actual readers - {count}");

            foreach (Reservation reserve in reservations)
            {
                ValidationResult reserveResult = _reservationValidator.Validate(reserve);
                ExceptionExtensions.Check<OLBadRequest>(!reserveResult.IsValid, $"The reserve has been changed incorrectly. Reservation info {reserve}");
                ExceptionExtensions.Check<OLBadRequest>(reserve.UserId < 1 || !(await _unitOfWork.UserRepository.IsUserExistAsync(reserve.UserId)), $"UserId incorrect {reserve.UserId}");
            }

            Reservation[] reservationsArray = reservations.ToArray();

            for (int i = 0; i < reservationsArray.Length - 1; i++)
            {
                if (reservationsArray[i].ReturnDate == null) reservationsArray[i].ReturnDate = DateTime.UtcNow;

                for (int y = i + 1; y < reservationsArray.Length; y++)
                {
                    if (reservationsArray[y].ReservationDate >= reservationsArray[i].ReturnDate || reservationsArray[y].ReturnDate <= reservationsArray[i].ReservationDate) continue;
                    ExceptionExtensions.Check<OLBadRequest>(true, $"dates overlap range1 - {reservationsArray[i].ReservationDate} - {reservationsArray[i].ReturnDate}, range2 - {reservationsArray[y].ReservationDate} - {reservationsArray[y].ReturnDate}");
                }
            }
        }

        public async Task<int> CreateBookAsync(Book book)
        {
            await _unitOfWork.BookRepository.CreateBookAsync(book);
            ExceptionExtensions.Check<OLInternalServerError>(book.Id == 0, "The book was not created");
            return book.Id;
        }

    }
}
