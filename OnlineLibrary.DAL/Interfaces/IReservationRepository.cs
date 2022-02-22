using OnlineLibrary.Common.DBEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineLibrary.DAL.Interfaces
{
    public interface IReservationRepository
    {
        Task<List<Reservation>> GetAllReservationsAsync();

        Task<List<Reservation>> GetBookReservationHistoryAsync(int bookId);

        Task<List<Reservation>> GetUserReservationHistoryAsync(int userId);

        Task CreateReservationAsync(Reservation reservation);

        Task CloseReservationAsync(Reservation reservation);

        Task<Reservation> GetBookReservationLastRow(int bookId);

    }
}
