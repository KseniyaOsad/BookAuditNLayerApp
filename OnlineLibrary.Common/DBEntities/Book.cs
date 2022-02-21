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

        public bool InArchive { get; set; } = false;

        public Genre Genre { get; set; }

        public List<Author> Authors { get; set; }

        public List<Tag> Tags { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        public override bool Equals(object obj)
        {
            Book book = obj as Book;
            if (book == null) return false;
            return book.Id == Id;
        }
    }
}
