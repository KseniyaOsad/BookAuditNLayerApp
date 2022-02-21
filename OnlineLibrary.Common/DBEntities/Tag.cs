using System.Collections.Generic;

namespace OnlineLibrary.Common.DBEntities
{
    public class Tag 
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<Book> Books { get; set; }

        public override bool Equals(object obj)
        {
            Tag tag = obj as Tag;
            if (tag == null) return false;
            return tag.Id == Id;
        }
    }
}
