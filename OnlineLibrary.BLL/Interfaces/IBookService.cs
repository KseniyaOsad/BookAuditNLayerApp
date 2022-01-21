using Microsoft.AspNetCore.JsonPatch;
using OnlineLibrary.Common.Entities;
using System.Collections.Generic;

namespace OnlineLibrary.BLL.Interfaces
{
    public interface IBookService
    {
        List<Book> GetAllBooks();

        List<Book> FilterBooks(int? authorId, string name, int? inReserve, int? inArchieve);

        Book GetBookById(int? bookId);

        void ChangeBookReservation(int? bookId, bool newReservationValue);

        void ChangeBookArchievation(int? bookId, bool newArchievationValue);

        int CreateBook(Book book);

        void UpdatePatch(int bookId, JsonPatchDocument<Book> book);
    }
}
