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

namespace OnlineLibrary.BLL.Services
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IValidator<Book> _bookValidator;

        public BookService(IUnitOfWork uow, IValidator<Book> validator)
        {
            _unitOfWork = uow;
            _bookValidator = validator;
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

        public async Task UpdatePatchAsync(int bookId, JsonPatchDocument<Book> book)
        {
            var originalBook = await _unitOfWork.BookRepository.GetBookByIdAsync(bookId);
            ExceptionExtensions.Check<OLNotFound>(originalBook == null, "Book not found");
            book.ApplyTo(originalBook);
            ExceptionExtensions.Check<OLBadRequest>(bookId != originalBook.Id, "You are trying to change the ID. it's not allowed.");

            ValidationResult results = _bookValidator.Validate(originalBook);
            ExceptionExtensions.Check<OLBadRequest>(!results.IsValid, "The book has been changed incorrectly");
            
            bool updateBook = false, updateAuthors = false, updateTags = false;

            // Take real authors and tags if needed.
            if (book.Operations.Any(x => x.path.Contains("authors")))
            {
                originalBook.Authors = await _unitOfWork.AuthorRepository.GetAuthorsByIdListAsync(originalBook.Authors.Select(x => x.Id).ToList());
                ExceptionExtensions.Check<OLBadRequest>(!originalBook.Authors.Any(), "You are trying to change the Authors. But this authors don't exists.");
                updateAuthors = true;
            }
            if (book.Operations.Any(x => x.path.Contains("tags")))
            {
                originalBook.Tags = await _unitOfWork.TagRepository.GetTagsByIdListAsync(originalBook.Tags.Select(x => x.Id).ToList());
                ExceptionExtensions.Check<OLBadRequest>(!originalBook.Tags.Any(), "You are trying to change the Tags. But this tags don't exists.");
                updateTags = true;
            }
            if (book.Operations.Any(x => !x.path.Contains("tags") && !x.path.Contains("authors")))
            {
                if (book.Operations.Any(x => x.path.Contains("inArchive")))
                {
                    Reservation reservationRow = await _unitOfWork.ReservationRepository.GetBookReservationLastRow(bookId);
                    ExceptionExtensions.Check<OLBadRequest>(reservationRow != null && reservationRow.ReturnDate == default, "You can't archivate this book. It is in reserve.");
                }
                updateBook = true;
            }

            await _unitOfWork.BookRepository.UpdateBookAsync(originalBook, updateBook, updateAuthors, updateTags);
        }

        public async Task<int> CreateBookAsync(Book book)
        {
            await _unitOfWork.BookRepository.CreateBookAsync(book);
            ExceptionExtensions.Check<OLInternalServerError>(book.Id == 0, "The book was not created");
            return book.Id;
        }
    }
}
