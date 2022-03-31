using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.Common.DBEntities.Enums;
using System.Collections.Generic;

namespace OnlineLibrary.DAL.DTO
{
    public class BookDTO
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public bool? InArchive { get; set; }

        public Genre? Genre { get; set; }

        public List<Author> Authors { get; set; }

        public List<Tag> Tags { get; set; }

        public List<Reservation> Reservations { get; set; }
    }
}
