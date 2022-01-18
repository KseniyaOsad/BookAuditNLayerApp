﻿using System.Collections.Generic;

namespace OnlineLibrary.DAL.Interfaces
{
    public interface IBookRepository<T> where T : class
    {
        List<T> GetAllBooks();

        List<T> FilterBooks(string bookName);

        List<T> FilterBooks(int authorId);

        List<T> FilterBooks(int authorId, string bookName);

        T GetBookById(int bookId);

        void ChangeBookReservation(int bookId, bool newReservationValue);

        void ChangeBookArchievation(int bookId, bool newArchievationValue);

        bool IsBookIdExists(int bookId);

        void CreateBook(T book);
    }
}
