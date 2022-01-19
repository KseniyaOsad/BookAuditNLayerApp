using OnlineLibrary.Common.Entities;
using System.Collections.Generic;

namespace OnlineLibrary.DAL.Interfaces
{
    public interface IBookRepository
    {
        List<Book> GetAllBooks();

        List<Book> FilterBooks(string bookName);

        List<Book> FilterBooks(int authorId);

        List<Book> FilterBooks(int authorId, string bookName);

        Book GetBookById(int bookId);

        void ChangeBookReservation(int bookId, bool newReservationValue);

        void ChangeBookArchievation(int bookId, bool newArchievationValue);

        bool IsBookIdExists(int bookId);

        void InsertBook(Book book);
    }
}
