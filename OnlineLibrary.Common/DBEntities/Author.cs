using System.Collections.Generic;

namespace OnlineLibrary.Common.DBEntities
{
    public class Author
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<Book> Books { get; set; }

        public override bool Equals(object obj)
        {
            Author author = obj as Author;
            if (author == null) return false;
            return author.Id == Id;
        }
    }
}
