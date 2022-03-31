using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.Common.EntityProcessing;
using OnlineLibrary.Common.EntityProcessing.Pagination;
using OnlineLibrary.DAL.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineLibrary.DAL.Interfaces
{
    public interface IBookRepository
    {
        Task<List<Book>> GetAllBooksForCsvAsync();

        Task<Book> GetBookByIdAsync(int bookId);

        Task CreateBookAsync(Book book);

        Task UpdateBookAsync(BookDTO book);

        Task<PaginatedList<Book>> FilterSortPaginBooksAsync(BookProcessing bookProcessing);

    }
}
