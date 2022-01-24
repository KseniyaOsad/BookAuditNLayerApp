using OnlineLibrary.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OnlineLibrary.DAL.Interfaces
{
    public interface IBookRepository
    {
        int GetAllBooksCount();

        int GetAllBooksCount(Expression<Func<Book, bool>> expr);

        List<Book> GetAllBooks();

        List<Book> GetAllBooks(int skip, int pageSize);

        List<Book> FilterBooks(Expression<Func<Book, bool>> expr);

        List<Book> FilterBooks(Expression<Func<Book, bool>> expr, int skip, int pageSize);

        Book GetBookById(int bookId);

        void ChangeBookReservation(int bookId, bool newReservationValue);

        void ChangeBookArchievation(int bookId, bool newArchievationValue);

        bool IsBookIdExists(int bookId);

        void InsertBook(Book book);
    }
}
