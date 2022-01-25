﻿using OnlineLibrary.Common.Pagination;

namespace OnlineLibrary.Common.Entities
{
    public class FilterBook
    {
        public string Name { get; set; }

        public int? AuthorId { get; set; }

        public int? TagId { get; set; }

        public int? Archievation { get; set; }

        public int? Reservation { get; set; }

        public PaginationOptions Pagination { get; set; }
    }
}
