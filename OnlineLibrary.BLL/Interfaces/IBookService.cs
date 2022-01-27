using Microsoft.AspNetCore.JsonPatch;
using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.Common.EntityProcessing;
using OnlineLibrary.Common.EntityProcessing.Pagination;

namespace OnlineLibrary.BLL.Interfaces
{
    public interface IBookService
    {
        PaginatedList<Book> GetAllBooks(PaginationOptions paginationOptions);

        PaginatedList<Book> FilterBooks(BookProcessing bookProcessing);

        Book GetBookById(int? bookId);

        int CreateBook(Book book);

        void UpdatePatch(int bookId, JsonPatchDocument<Book> book);
    }
}
