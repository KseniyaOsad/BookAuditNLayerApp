using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineLibrary.Common.Pagination
{
    public class PaginationOptions
    {
        const int maxPageSize = 50;

        const int minPageSize = 2;

        private int pageNumber = 1;

        private int pageSize = 10;

        public PaginationOptions(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public int PageNumber
        {
            get
            {
                return pageNumber;
            }
            set
            {
                pageNumber = (value <= 0) ? 1 : value;
            }
        }

        public int PageSize
        {
            get
            {
                return pageSize;
            }
            set
            {
                pageSize =
                    (value > maxPageSize) ?
                        maxPageSize : (value < minPageSize) ?
                            minPageSize : value;
            }
        }
    }
}
