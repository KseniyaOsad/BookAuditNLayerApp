
using BookAuditNLayer.GeneralClassLibrary.Entities;
using BookAuditNLayerApp.DAL.EF;
using BookAuditNLayerApp.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAuditNLayerApp.DAL.Repositories
{
    public class BookRepository : IBookRepository<Book>
    {
        BookContext _context;

        public BookRepository(BookContext context)
        {
            _context = context;
        }

        public void ChangeBookArchievation(int bookId, bool newArchievationValue)
        {
            Book book = _context.Book.Where(b => b.Id == bookId).First();
            book.InArchive = newArchievationValue;
            _context.SaveChanges();
        }

        public void ChangeBookReservation(int bookId, bool newReservationValue)
        {
            Book book = _context.Book.Where(b => b.Id == bookId).First();
            book.Reserve = newReservationValue;
            _context.SaveChanges();
        }

        public int CreateBook(Book book)
        {
            _context.Add(book);
            _context.SaveChanges();
            return book.Id;
        }

        public List<Book> FilterBooks(int authorId)
        {
            return _context.Book.Include(b => b.Author)
                .Where(b => b.Author.Id == authorId)
                .ToList();
        }

        public List<Book> FilterBooks(string name)
        {
            return _context.Book.Include(b => b.Author)
                .Where(b => b.Name.ToLower().Contains(name.ToLower()))
                .ToList();
        }
        public List<Book> FilterBooks(int authorId, string name)
        {
            return _context.Book.Include(b => b.Author)
                .Where(b => b.Name.ToLower().Contains(name.ToLower()) && b.Author.Id == authorId)
                .ToList();
        }

        public List<Book> GetAllBooks()
        {
            return _context.Book.Include(b => b.Author)
                .OrderBy(b => b.Name)
                .ToList();
        }

        public Book GetBookById(int bookId)
        {
            return _context.Book.Include(b => b.Author)
                .Where(b => b.Id == bookId)
                .First();
        }

        public bool IsBookIdExists(int bookId)
        {
            return _context.Book.Any(b => b.Id == bookId);
        }
    }
}
