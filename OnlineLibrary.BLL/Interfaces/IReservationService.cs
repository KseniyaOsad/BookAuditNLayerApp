using OnlineLibrary.Common.DBEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineLibrary.BLL.Interfaces
{
    public interface IReservationService
    {
        Task<List<Reservation>> GetAllReservationsAsync();

        Task<int> CreateReservationAsync(Reservation reservation); 

        Task CloseReservationAsync(Reservation reservation); 
    }
}
