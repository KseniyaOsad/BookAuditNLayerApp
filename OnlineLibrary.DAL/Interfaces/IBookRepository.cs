using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.Common.EntityProcessing;
using OnlineLibrary.Common.EntityProcessing.Pagination;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OnlineLibrary.DAL.Interfaces
{
    public interface IBookRepository
    {
        Task<int> GetAllBooksCountAsync();

        Task<int> GetAllBooksCountAsync(Expression<Func<Book, bool>> expr);

        Task<List<Book>> GetAllBooksAsync(int skip, int pageSize);

        Task<List<Book>> FilterBooksAsync(Expression<Func<Book, bool>> expr, int skip, int pageSize, string propertyToOrder, ListSortDirection SortDirection);

        Task<Book> GetBookByIdAsync(int bookId);

        void InsertBook(Book book);

        Task<PaginatedList<Book>> FilterBooksAsync(BookProcessing bookProcessing);
    }
}
