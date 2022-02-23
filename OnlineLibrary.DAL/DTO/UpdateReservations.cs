using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.DAL.DTO.Enums;

namespace OnlineLibrary.DAL.DTO
{
    public class UpdateReservations
    {
        public UpdateReservations(Reservation reservation, UpdateStatus status)
        {
            Reservation = reservation;
            Status = status;
        }

        public Reservation Reservation { get; set; }

        public UpdateStatus Status { get; set; }
    }
}
