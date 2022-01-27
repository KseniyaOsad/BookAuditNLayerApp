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
        public PaginationOptions Pagination { get; set; }

        public SortingOptions Sorting { get; set; }

        public BookFiltration Filtration { get; set; }

        public void MakeValid()
        {
            Pagination = Pagination ?? new PaginationOptions();
            Sorting = Sorting ?? new SortingOptions();
            Filtration = Filtration ?? new BookFiltration();
        }
    }
}
