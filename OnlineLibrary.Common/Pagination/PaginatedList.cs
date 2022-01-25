using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineLibrary.Common.Pagination
{
    public class PaginatedList<T>
    {
        public PaginatedList()
        {
            TotalCount = 0;
            Results = new List<T>();
        }

        public PaginatedList(int totalCount, List<T> results)
        {
            TotalCount = totalCount;
            Results = results;
        }

        public int TotalCount { get; set; } = 0;

        public List<T> Results { get; set; } = new List<T>();
    }
}
