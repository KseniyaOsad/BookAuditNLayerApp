﻿using OnlineLibrary.Common.EntityProcessing.Pagination;
using System.ComponentModel;

namespace OnlineLibrary.Common.EntityProcessing.Filtration
{
    public class BookFiltration
    {
        public string Name { get; set; }

        public int? AuthorId { get; set; }

        public int? TagId { get; set; }

        public int? Archievation { get; set; }

        public int? Reservation { get; set; }

    }
}