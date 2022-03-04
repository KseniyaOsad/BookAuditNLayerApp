using System;

namespace OnlineLibrary.DAL.DTO
{
    public class ReservationDTO
    {
        public int? Id { get; set; }

        public int UserId { get; set; }

        public int BookId { get; set; }

        public DateTime ReservationDate { get; set; }

        public DateTime? ReturnDate { get; set; }
    }
}
