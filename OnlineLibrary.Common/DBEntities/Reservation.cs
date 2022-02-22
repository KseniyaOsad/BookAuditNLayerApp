using System;
using System.Text;

namespace OnlineLibrary.Common.DBEntities
{
    public class Reservation
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        
        public User User { get; set; }

        public int BookId { get; set; }

        public Book Book { get; set; }

        public DateTime ReservationDate { get; set; } = DateTime.Now;

        public DateTime? ReturnDate { get; set; }

        public override string ToString()
        {
            string returnDate = ReturnDate == default ? "not returned" : ReturnDate.ToString();
            return new StringBuilder(Id.ToString()).Append(",")
                .Append(Book?.Id).Append(",")
                .Append(Book?.Name).Append(",")
                .Append(User?.Id).Append(",")
                .Append(User?.Name).Append(",")
                .Append(ReservationDate).Append(",")
                .Append(returnDate).Append("\n")
                .ToString();
        }
    }
}
