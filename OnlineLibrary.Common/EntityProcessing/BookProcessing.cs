using OnlineLibrary.Common.EntityProcessing.Filtration;
using OnlineLibrary.Common.EntityProcessing.Pagination;
using OnlineLibrary.Common.EntityProcessing.Sorting;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineLibrary.Common.EntityProcessing
{
    public class BookProcessing
    {
        public PaginationOptions Pagination { get; set; } = new PaginationOptions();

        public SortingOptions Sorting { get; set; } = new SortingOptions();

        public BookFiltration Filtration { get; set; } = new BookFiltration();

    }
}
