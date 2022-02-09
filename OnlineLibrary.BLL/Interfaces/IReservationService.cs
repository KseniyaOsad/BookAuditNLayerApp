
using Microsoft.AspNetCore.JsonPatch;
using OnlineLibrary.Common.DBEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineLibrary.BLL.Interfaces
{
    public interface IReservationService
    {
        Task<List<Reservation>> GetAllReservationsAsync();

        Task UpdatePatchAsync(int reservationId, JsonPatchDocument<Reservation> reservation);

        Task<int> CreateReservationAsync(Reservation reservation); 

        Task CloseReservationAsync(Reservation reservation); 
    }
}
