using System;
using System.Collections.Generic;
using System.Text;

namespace BookAuditNLayerApp.BLL.DTO
{
    public class BookDTO
    {
        // Планировалось, что будет использоваться в этом проекте в качестве альтернативы для класса Book,
        // но так как он полностью копирует класс Book, он вскоре будет изменен или удален из проекта совсем 
        // Вместо него будет использоваться класс Book, который был перенесен в папку Entities в проекте GeneralClassLibrary.

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int AuthorId { get; set; }

        public bool Reserve { get; set; }

        public bool InArchive { get; set; }
    }
}
