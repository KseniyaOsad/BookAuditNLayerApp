using OnlineLibrary.BLL.Interfaces;
using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.Common.Exceptions;
using OnlineLibrary.Common.Extensions;
using OnlineLibrary.DAL.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineLibrary.BLL.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReservationService(IUnitOfWork uow)
        {
            _unitOfWork = uow;
        }

        public async Task CloseReservationAsync(Reservation reservation)
        {
            Reservation reservationRow = await _unitOfWork.ReservationRepository.GetBookReservationLastRow(reservation.Book.Id);
            ExceptionExtensions.Check<OLNotFound>(reservationRow == null || reservationRow.UserId != reservation.User.Id, $"Reservation not found.");
            ExceptionExtensions.Check<OLBadRequest>(reservationRow.ReturnDate != default, $"Book isn't in reserve.");
            await _unitOfWork.ReservationRepository.CloseReservationAsync(reservation);
        }

        public async Task<int> CreateReservationAsync(Reservation reservation)
        {
            ExceptionExtensions.Check<OLBadRequest>(!(await _unitOfWork.UserRepository.IsUserExistAsync(reservation.User.Id)), $"User not found. User id = {reservation.User.Id}");

            int bookId = reservation.Book.Id;
            reservation.Book = await _unitOfWork.BookRepository.GetBookByIdAsync(reservation.Book.Id);
            ExceptionExtensions.Check<OLNotFound>(reservation.Book == null, $"Book not found. Book id = {bookId}");
            ExceptionExtensions.Check<OLBadRequest>(reservation.Book.InArchive, $"Book is in Archive. Book id = {bookId}");

            Reservation reservationRow = await _unitOfWork.ReservationRepository.GetBookReservationLastRow(bookId);
            ExceptionExtensions.Check<OLBadRequest>(reservationRow != null && reservationRow.ReturnDate == default, $"Book in reserve.");

            await _unitOfWork.ReservationRepository.CreateReservationAsync(reservation);
            ExceptionExtensions.Check<OLInternalServerError>(reservation.Id == 0, "The reservation was not created");
            return reservation.Id;
        }

        public Task<List<Reservation>> GetAllReservationsAsync()
        {
            return  _unitOfWork.ReservationRepository.GetAllReservationsAsync();
        }
    }
}
