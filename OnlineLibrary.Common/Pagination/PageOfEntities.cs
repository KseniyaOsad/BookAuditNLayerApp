using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineLibrary.Common.Pagination
{
    public class PageOfEntities<T>
    {
        public int Page { get; set; } = 1;

        public int ElementsOnPage { get; set; } = 0;

        public int EntitiesCount { get; set; } = 0;

        public List<T> Entities { get; set; } = new List<T>();

    }
}
