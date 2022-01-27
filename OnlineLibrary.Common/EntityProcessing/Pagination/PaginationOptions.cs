namespace OnlineLibrary.Common.EntityProcessing.Pagination
{
    public class PaginationOptions
    {
        const int minPageSize = 2;

        private int pageNumber;

        private int pageSize;

        public PaginationOptions(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public PaginationOptions()
        {
            PageNumber = 1;
            PageSize = 10;
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
                     (value < minPageSize) ?
                            minPageSize : value;
            }
        }
    }
}
