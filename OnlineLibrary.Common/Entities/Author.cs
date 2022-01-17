using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineLibrary.Common.Entities
{
    public class Author
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<Book> Books { get; set; }
    }
}
