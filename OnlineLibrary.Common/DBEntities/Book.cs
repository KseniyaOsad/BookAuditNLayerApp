using OnlineLibrary.Common.DBEntities.Enums;
using System;
using System.Collections.Generic;

namespace OnlineLibrary.Common.DBEntities
{
    public class Book
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool Reserve { get; set; } = false;

        public bool InArchive { get; set; } = false;

        public Genre Genre { get; set; }

        public List<Author> Authors { get; set; }

        public List<Tag> Tags { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.Now;
    }
}
