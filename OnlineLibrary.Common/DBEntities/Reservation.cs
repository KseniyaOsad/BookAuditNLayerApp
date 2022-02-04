using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineLibrary.Common.DBEntities
{
    public class Reservation
    {
        public int Id { get; set; }

        public User User { get; set; }

        public Book Book { get; set; }

        public bool IsBookReturned { get; set; } = false;

        public DateTime ReservationDate { get; set; } = DateTime.Now;

        public DateTime? ReturnDate { get; set; }
    }
}
