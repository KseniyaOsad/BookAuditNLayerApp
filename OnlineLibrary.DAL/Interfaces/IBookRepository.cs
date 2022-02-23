using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.Common.EntityProcessing.Filtration;
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

        Task UpdateBookAsync(Book book);

        Task<List<int>> GetAuthorIdsByBookAsync(int bookId);

        Task UpdateAuthorBookAsync(Book book, List<int> newAuthors, List<int> oldAuthors);

        Task<List<int>> GetTagIdsByBookAsync(int bookId);

        Task UpdateBookTagAsync(Book book, List<int> newTags, List<int> oldTags);

        Task<List<int>> FilterBooksAsync(BookFiltration bookFiltration);

        Task<List<Book>> SortPaginBooksAsync(List<int> bookIds, bool fromBooks, SortingOptions sortingOptions, int skip, int pageSize);

        Task<Book> GetBookInfoAndBookReservationsAsync(int bookId);

        Task UpdateBookWithReservations(Book book);
    }
}
