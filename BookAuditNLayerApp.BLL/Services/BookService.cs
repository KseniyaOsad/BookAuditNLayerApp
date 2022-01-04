using BookAuditNLayer.GeneralClassLibrary.Entities;
using BookAuditNLayer.GeneralClassLibrary.Enums;
using BookAuditNLayerApp.BLL.DTO;
using BookAuditNLayerApp.BLL.Infrastructure;
using BookAuditNLayerApp.BLL.Interfaces;
using BookAuditNLayerApp.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookAuditNLayerApp.BLL.Services
{
    public class BookService : IBookService<Book>
    {
        IUnitOfWork Database { get; set; }
        public BookService(IUnitOfWork uow)
        {
            Database = uow;
        }

        public List<Book> GetAllBooks()
        {
            List<Book> books = Database.Book.GetAllBooks();
            if (books.Count == 0 || books == null)
            {
                throw new ValidationException("Книг не существует", ErrorList.ListIsEmpty);
            }
            return books;
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

        public Book GetBookById(int bookId)
        {
            throw new NotImplementedException();
        }

        public void ChangeBookReservation(int bookId, bool newReservationValue)
        {
            throw new NotImplementedException();
        }

        public void ChangeBookArchievation(int bookId, bool newArchievationValue)
        {
            throw new NotImplementedException();
        }

        public bool IsBookIdExists(int bookId)
        {
            throw new NotImplementedException();
        }

        public void CreateBook(Book book)
        {
            throw new NotImplementedException();
        }
    }
}
