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
using OnlineLibrary.DAL.DTO;
using OnlineLibrary.DAL.DTO.Enums;
using System;

namespace OnlineLibrary.BLL.Services
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IValidator<Book> _bookValidator;


        public BookService(IUnitOfWork uow, IValidator<Book> bookValidator)
        {
            _unitOfWork = uow;
            _bookValidator = bookValidator;
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

        private async Task UpdateAuthorBookAsync(Book book)
        {
            List<int> oldAuthors = await _unitOfWork.BookRepository.GetAuthorIdsByBookAsync(book.Id);

            List<int> newAuthors = book.Authors.Select(a => a.Id).ToList();

            foreach (var newAuthor in book.Authors)
            {
                if (oldAuthors.Contains(newAuthor.Id))
                {
                    oldAuthors.Remove(newAuthor.Id);
                    newAuthors.Remove(newAuthor.Id);
                }
            }

            if (oldAuthors.Any() || newAuthors.Any())
                await _unitOfWork.BookRepository.UpdateAuthorBookAsync(book, newAuthors, oldAuthors);
        }

        private async Task UpdateBookTagAsync(Book book)
        {
            List<int> oldTags = await _unitOfWork.BookRepository.GetTagIdsByBookAsync(book.Id);

            List<int> newTags = book.Tags.Select(t => t.Id).ToList();

            foreach (var newTag in book.Tags)
            {
                if (oldTags.Contains(newTag.Id))
                {
                    oldTags.Remove(newTag.Id);
                    newTags.Remove(newTag.Id);
                }
            }
            if (oldTags.Any() || newTags.Any())
                await _unitOfWork.BookRepository.UpdateBookTagAsync(book, newTags, oldTags);
        }

        public async Task UpdatePatchAsync(int bookId, JsonPatchDocument<Book> book)
        {
            ExceptionExtensions.Check<OLInternalServerError>(!book.Operations.All(x => x.op.Contains("REPLACE")), "this method supports only replace operations");

            var originalBook = await _unitOfWork.BookRepository.GetBookByIdAsync(bookId);
            ExceptionExtensions.Check<OLNotFound>(originalBook == null, "Book not found");
            book.ApplyTo(originalBook);
            ExceptionExtensions.Check<OLBadRequest>(bookId != originalBook.Id, "You are trying to change the ID. it's not allowed.");

            ValidationResult results = _bookValidator.Validate(originalBook);
            ExceptionExtensions.Check<OLBadRequest>(!results.IsValid, "The book has been changed incorrectly");
            
            // update AuthorBook if need
            if (book.Operations.Any(x => x.path.Contains("authors")))
            {
                originalBook.Authors = await _unitOfWork.AuthorRepository.GetAuthorsByIdListAsync(originalBook.Authors.Select(x => x.Id).ToList());
                ExceptionExtensions.Check<OLBadRequest>(!originalBook.Authors.Any(), "You are trying to change the Authors. But this authors don't exists.");
                await UpdateAuthorBookAsync(originalBook);
            }

            // update BookTag if need
            if (book.Operations.Any(x => x.path.Contains("tags")))
            {
                originalBook.Tags = await _unitOfWork.TagRepository.GetTagsByIdListAsync(originalBook.Tags.Select(x => x.Id).ToList());
                ExceptionExtensions.Check<OLBadRequest>(!originalBook.Tags.Any(), "You are trying to change the Tags. But this tags don't exists.");
                await UpdateBookTagAsync(originalBook);
            }

            // update book info if need
            if (book.Operations.Any(x => !x.path.Contains("tags") && !x.path.Contains("authors")))
            {
                if (book.Operations.Any(x => x.path.Contains("inArchive")))
                {
                    Reservation reservationRow = await _unitOfWork.ReservationRepository.GetBookReservationLastRow(bookId);
                    ExceptionExtensions.Check<OLBadRequest>(reservationRow != null && reservationRow.ReturnDate == default, "You can't archivate this book. It is in reserve.");
                }
                await _unitOfWork.BookRepository.UpdateBookAsync(originalBook);
            }
        }

        public async Task<int> CreateBookAsync(Book book)
        {
            await _unitOfWork.BookRepository.CreateBookAsync(book);
            ExceptionExtensions.Check<OLInternalServerError>(book.Id == 0, "The book was not created");
            return book.Id;
        }

        public async Task UpdatePatchReservationAsync(int bookId, JsonPatchDocument<Book> bookJson)
        {
            ExceptionExtensions.Check<OLInternalServerError>(!bookJson.Operations.All(x => x.op.Contains("REPLACE")), "this method supports only replace operations");
            ExceptionExtensions.Check<OLNotFound>(!bookJson.Operations.Any(x => x.path.Contains("reservations")), "this test method only for updating reservations");
            var book = await _unitOfWork.BookRepository.GetBookInfoAndBookReservationsAsync(bookId);
            ExceptionExtensions.Check<OLNotFound>(book == null, "Book not found");
            List<Reservation> actualReservations = book.Reservations;
            bookJson.ApplyTo(book);

            ExceptionExtensions.Check<OLBadRequest>(book.Reservations.Any(r=>r.BookId != default && r.BookId != bookId), $"You are trying to edit bookID it's not allowed, actual bookId = {bookId} ");

            List<UpdateReservations> updateReservations = new List<UpdateReservations>();

            foreach (Reservation reserve in book.Reservations)
            {
                reserve.BookId = bookId;

                // check if we trying to update or keep record
                if (actualReservations != null && reserve.Id != default )
                {
                    // try to find record
                    Reservation sameReservation = actualReservations.FirstOrDefault(r => r.Id == reserve.Id && r.BookId == reserve.BookId);
                    ExceptionExtensions.Check<OLBadRequest>(sameReservation == null, $"You are trying to edit an record, but the record (with id = {reserve.Id} and bookId = {reserve.BookId}) does not exist");

                    // make valid/check if valid
                    reserve.ReservationDate = reserve.ReservationDate == default ? sameReservation.ReservationDate : reserve.ReservationDate;
                    reserve.UserId = reserve.UserId == default ? sameReservation.UserId : reserve.UserId;
                    ExceptionExtensions.Check<OLBadRequest>(reserve.UserId != sameReservation.UserId, $"You are trying to edit a user ({reserve.UserId}) it's not allowed. Actual = {sameReservation.UserId} ");

                    // Record exist. Check status - update or keep
                    if (sameReservation.Equals(reserve))
                        updateReservations.Add(new UpdateReservations(reserve, UpdateStatus.Keep));
                    else
                        updateReservations.Add(new UpdateReservations(reserve, UpdateStatus.Update));
                    // delete from actualReservations. whatever remains will be deleted 
                    actualReservations.Remove(sameReservation);
                }
                else
                {
                    ExceptionExtensions.Check<OLBadRequest>(reserve.Id != default, $"You are trying to edit an record, but the record (with id = {reserve.Id} and bookId = {reserve.BookId}) does not exist");
                    ExceptionExtensions.Check<OLBadRequest>(reserve.UserId < 1 || !(await _unitOfWork.UserRepository.IsUserExistAsync(reserve.UserId)), $"UserId incorrect {reserve.UserId}");
                    reserve.ReservationDate = reserve.ReservationDate == default ? DateTime.UtcNow : reserve.ReservationDate;
                    updateReservations.Add(new UpdateReservations(reserve, UpdateStatus.Create));
                }

                ExceptionExtensions.Check<OLBadRequest>(reserve.ReturnDate != null && reserve.ReturnDate < reserve.ReservationDate, $"Return date {reserve.ReturnDate} less then reservation date {reserve.ReservationDate}");
            }

            foreach (Reservation reserve in actualReservations)
            {
                updateReservations.Add(new UpdateReservations(reserve, UpdateStatus.Delete));
            }

            ExceptionExtensions.Check<OLBadRequest>(updateReservations.All(x => x.Status == UpdateStatus.Keep), $"nothing has changed");

            VerifyIfUpdatedReservationsIsValid(updateReservations);
            await _unitOfWork.ReservationRepository.UpdateBookReservationsAsync(updateReservations);
        }

        private void VerifyIfUpdatedReservationsIsValid(List<UpdateReservations> updateReservations)
        {
            int count = updateReservations.Where(r => r.Reservation.ReturnDate == null && r.Status != UpdateStatus.Delete).Count();
            ExceptionExtensions.Check<OLBadRequest>(count > 1, $"Only 1 person can read the book at a time. Actual readers - {count}");

            Reservation[] reservations = updateReservations.Where(r => r.Status != UpdateStatus.Delete).Select(r => r.Reservation).ToArray();

            for (int i = 0; i < reservations.Length - 1; i++)
            {
                if (reservations[i].ReturnDate == null) reservations[i].ReturnDate = DateTime.UtcNow;

                for (int y = i + 1; y < reservations.Length; y++)
                {
                    if (reservations[y].ReservationDate >= reservations[i].ReturnDate || reservations[y].ReturnDate <= reservations[i].ReservationDate) continue;
                    ExceptionExtensions.Check<OLBadRequest>(true, $"dates overlap range1 - {reservations[i].ReservationDate} - {reservations[i].ReturnDate}, range2 - {reservations[y].ReservationDate} - {reservations[y].ReturnDate}");
                }
            }
        }

        public async Task<Book> GetBookInfoAndBookReservationsAsync(int bookId)
        {
            Book book = await _unitOfWork.BookRepository.GetBookInfoAndBookReservationsAsync(bookId);
            ExceptionExtensions.Check<OLNotFound>(book == null, "Book not found");
            return book;
        }
    }
}
