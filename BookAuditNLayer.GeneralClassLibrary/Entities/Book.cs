using System;

namespace BookAuditNLayer.GeneralClassLibrary.Entities
{
    public class Book
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int AuthorId { get; set; }

        public Author Author { get; set; }
        // В данном случае у книги может быть только один автор.

        public bool Reserve { get; set; }

        public bool InArchive { get; set; }
    }
}
