using OnlineLibrary.Common.Entities;
using OnlineLibrary.Common.Pagination;
using OnlineLibrary.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OnlineLibraryApiTest.Repositories
{
    class TestBookRepository : IBookRepository
    {
        public void ChangeBookArchievation(int bookId, bool newArchievationValue)
        {
        }

        public void ChangeBookReservation(int bookId, bool newReservationValue)
        {
        }

        public void InsertBook(Book book)
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

        public List<Book> GetAllBooks(PaginationOptions paginationOptions)
        {
            throw new NotImplementedException();
        }

        public int GetAllBooksCount()
        {
            throw new NotImplementedException();
        }

        public List<Book> GetAllBooks(int skip, int pageSize)
        {
            throw new NotImplementedException();
        }

        public List<Book> FilterBooks(Expression<Func<Book, bool>> expr)
        {
            throw new NotImplementedException();
        }
    }
}
