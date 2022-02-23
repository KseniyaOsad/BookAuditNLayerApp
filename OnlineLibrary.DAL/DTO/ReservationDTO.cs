using OnlineLibrary.Common.DBEntities;
using System;

namespace OnlineLibrary.DAL.DTO
{
    public class ReservationDTO
    {
        public ReservationDTO(Reservation reservation)
        {
            Id = reservation.Id;
            UserId = reservation.UserId;
            BookId = reservation.BookId;
            ReservationDate = reservation.ReservationDate;
            ReturnDate = reservation.ReturnDate;
        }

        public int? Id { get; set; }

        public int UserId { get; set; }

        public int BookId { get; set; }

        public DateTime ReservationDate { get; set; }

        public DateTime? ReturnDate { get; set; }
    }
}
