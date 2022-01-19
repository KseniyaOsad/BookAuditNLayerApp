using OnlineLibrary.Common.Entities;
using OnlineLibrary.DAL.Interfaces;
using System;
using System.Collections.Generic;

namespace OnlineLibraryApiTest.Repositories
{
    class TestBookRepository : IBookRepository<Book>
    {
        public void ChangeBookArchievation(int bookId, bool newArchievationValue)
        {
        }

        public void ChangeBookReservation(int bookId, bool newReservationValue)
        {
        }

        public void CreateBook(Book book)
        {
        }

        public List<Book> FilterBooks(string bookName)
        {
            throw new NotImplementedException();
        }

        public List<Book> FilterBooks(int authorId)
        {
            throw new NotImplementedException();
        }

        public List<Book> FilterBooks(int authorId, string bookName)
        {
            throw new NotImplementedException();
        }

        public List<Book> GetAllBooks()
        {
            throw new NotImplementedException();
        }

        public Book GetBookById(int bookId)
        {
            throw new NotImplementedException();
        }

        public bool IsBookIdExists(int bookId)
        {
            throw new NotImplementedException();
        }
    }
}
