using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.Common.EntityProcessing.Filtration;
using OnlineLibrary.Common.EntityProcessing.Sorting;
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

        Task<List<int>> FilterBooksAsync(BookFiltration bookFiltration);

        Task<List<Book>> SortPaginBooksAsync(List<int> bookIds, bool fromBooks, SortingOptions sortingOptions, int skip, int pageSize);
    }
}
