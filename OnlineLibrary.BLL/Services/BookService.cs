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
using AutoMapper;

namespace OnlineLibrary.BLL.Services
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IValidator<BookDTO> _bookDTOValidator;

        private readonly IValidator<Reservation> _reservationValidator;

        private readonly IMapper _mapper;

        public BookService(IUnitOfWork uow, IValidator<BookDTO> bookValidator,IValidator<Reservation> reservationValidator, IMapper mapper)
        {
            _unitOfWork = uow;
            _bookDTOValidator = bookValidator;
            _reservationValidator = reservationValidator;
            _mapper = mapper;
        }

        public async Task<PaginatedList<Book>> FilterSortPaginBooksAsync(BookProcessing bookProcessing)
        {
            PaginatedList<Book> result = await _unitOfWork.BookRepository.FilterSortPaginBooksAsync(bookProcessing);
            ExceptionExtensions.Check<OLNotFound>(result.TotalCount == 0, "Can't load paginated books. Books not found.");
            return result;
        }

        public async Task<Book> GetBookByIdAsync(int bookId)
        {
            Book book = await _unitOfWork.BookRepository.GetBookByIdAsync(bookId);
            ExceptionExtensions.Check<OLNotFound>(book == null, "Book not found");
            return book;
        }

        public async Task UpdatePatchAsync(int bookId, JsonPatchDocument<BookDTO> bookJson)
        {
            Book book = await _unitOfWork.BookRepository.GetBookByIdAsync(bookId);
            ExceptionExtensions.Check<OLNotFound>(book == null, "Book not found");
            BookDTO bookDTO = _mapper.Map<Book, BookDTO>(book);

            bookJson.ApplyTo(bookDTO);
            ExceptionExtensions.Check<OLBadRequest>(bookId != bookDTO.Id, "You are trying to change the ID. it's not allowed.");

            ValidationResult bookResult = _bookDTOValidator.Validate(bookDTO);
            ExceptionExtensions.Check<OLBadRequest>(!bookResult.IsValid, "The book has been changed incorrectly");

            // validate reservations 
            await CheckReservationValidation(bookId, bookDTO.Reservations);
            
            // check if we can change archivation
            if (bookJson.Operations.Any(x => x.path.ToUpper().Contains(nameof(Book.InArchive).ToUpper())))
            {
                ExceptionExtensions.Check<OLBadRequest>(bookDTO.InArchive == true && bookDTO.Reservations.Any(r => r.ReturnDate == null), "You can't archivate this book. It is in reserve.");
            }

            await _unitOfWork.BookRepository.UpdateBookAsync(bookDTO);
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
