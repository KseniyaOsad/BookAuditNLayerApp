using System.Collections.Generic;

namespace OnlineLibrary.Common.Entities
{
    public class Tag
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<Book> Books { get; set; }
    }
}
