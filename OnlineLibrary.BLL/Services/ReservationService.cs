using Microsoft.AspNetCore.JsonPatch;
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
            await _unitOfWork.ReservationRepository.CloseReservationAsync(reservation);
        }

        public async Task<int> CreateReservationAsync(Reservation reservation)
        {
            ExceptionExtensions.Check<OLBadRequest>(!(await _unitOfWork.UserRepository.IsUserExistAsync(reservation.User.Id)), $"User not found. User id = {reservation.User.Id}");
            
            reservation.Book = await _unitOfWork.BookRepository.GetBookByIdAsync(reservation.Book.Id);
            ExceptionExtensions.Check<OLNotFound>(reservation.Book == null, $"Book not found. Book id = {reservation.Book.Id}");
            ExceptionExtensions.Check<OLBadRequest>(reservation.Book.InArchive, $"Book is in Archive.");

            await _unitOfWork.ReservationRepository.CreateReservationAsync(reservation);
            ExceptionExtensions.Check<OLInternalServerError>(reservation.Id == 0, "The reservation was not created");
            return reservation.Id;
        }

        public async Task<List<Reservation>> GetAllReservationsAsync()
        {
            return await _unitOfWork.ReservationRepository.GetAllReservationsAsync();
        }

        public Task UpdatePatchAsync(int reservationId, JsonPatchDocument<Reservation> reservation)
        {
            throw new System.NotImplementedException();
        }
    }
}
