using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.Common.EntityProcessing;
using OnlineLibrary.Common.EntityProcessing.Pagination;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineLibrary.DAL.Interfaces
{
    public interface IBookRepository
    {
        Task<List<Book>> GetAllBooksForCsvAsync();

        Task<Book> GetBookByIdAsync(int bookId);

        Task CreateBookAsync(Book book);

        Task UpdateBookAsync(Book book, bool updateBook, bool updateAuthors, bool updateTags);

        Task<PaginatedList<Book>> FilterBooksAsync(BookProcessing bookProcessing);

        Task<bool> IsBookExistAsync(int userId);
    }
}
