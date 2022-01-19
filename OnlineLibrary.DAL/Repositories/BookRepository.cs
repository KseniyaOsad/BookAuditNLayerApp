using OnlineLibrary.Common.Entities;
using OnlineLibrary.DAL.EF;
using OnlineLibrary.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace OnlineLibrary.DAL.Repositories
{
    public class BookRepository : IBookRepository
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
        }

        public void ChangeBookReservation(int bookId, bool newReservationValue)
        {
            Book book = _context.Book.Where(b => b.Id == bookId).First();
            book.Reserve = newReservationValue;
        }

        public void InsertBook(Book book)
        {
            _context.Add(book);
        }

        public List<Book> FilterBooks(int authorId)
        {
            return _context.Book
                .Include(b => b.Authors.Where(a => a.Id == authorId))
                .ToList();
        }

        public List<Book> FilterBooks(string name)
        {
            return _context.Book
                .Include(b => b.Authors)
                .Where(b => b.Name.ToLower().Contains(name.ToLower()))
                .ToList();
        }
        public List<Book> FilterBooks(int authorId, string name)
        {
            return _context.Book
                .Include(b => b.Authors.Where(a => a.Id == authorId))
                .Where(b => b.Name.ToLower().Contains(name.ToLower()))
                .ToList();
        }

        public List<Book> GetAllBooks()
        {
            return _context.Book
                .Include(b => b.Authors)
                .OrderBy(b => b.Name)
                .ToList();
        }

        public Book GetBookById(int bookId)
        {
            return _context.Book
                .Include(b => b.Authors)
                .Where(b => b.Id == bookId)
                .First();
        }

        public bool IsBookIdExists(int bookId)
        {
            return _context.Book.Any(b => b.Id == bookId);
        }
    }
}
