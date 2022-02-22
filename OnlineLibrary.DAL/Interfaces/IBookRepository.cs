using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.Common.EntityProcessing;
using OnlineLibrary.Common.EntityProcessing.Filtration;
using OnlineLibrary.Common.EntityProcessing.Pagination;
using OnlineLibrary.Common.EntityProcessing.Sorting;
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

        Task<List<int>> FilterBooksAsync(BookFiltration bookFiltration);
       
        Task<List<Book>> SortPaginBooksAsync(List<int> bookIds, bool fromBooks, SortingOptions sortingOptions, int skip, int pageSize);
    }
}
