using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.DAL.EF;
using OnlineLibrary.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System;
using System.ComponentModel;
using OnlineLibrary.Common.Extensions;

namespace OnlineLibrary.DAL.Repositories
{
    public class BookRepository : IBookRepository
    {
        BookContext _context;

        public BookRepository(BookContext context)
        {
            _context = context;
        }

        public void InsertBook(Book book)
        {
            _context.Add(book);
        }

        public Book GetBookById(int bookId)
        {
            return _context.Book
                .Include(b => b.Authors)
                .Include(b => b.Tags)
                .FirstOrDefault(b => b.Id == bookId);
        }

        public bool IsBookIdExists(int bookId)
        {
            return _context.Book.Any(b => b.Id == bookId);
        }

        public List<Book> GetAllBooks(int skip, int pageSize)
        {
            return _context.Book
                    .Include(b => b.Tags)
                    .Include(b => b.Authors)
                    .Skip(skip)
                    .Take(pageSize)
                    .ToList();
        }

        public int GetAllBooksCount()
        {
            return _context.Book.Count();
        }

        public int GetAllBooksCount(Expression<Func<Book, bool>> expr)
        {
            return _context.Book
                .Where(expr)
                .Count();
        }

        public List<Book> FilterBooks(Expression<Func<Book, bool>> expr, int skip, int pageSize, string propertyToOrder , ListSortDirection SortDirection )
        {
            return _context.Book
                .Include(x => x.Tags)
                .Include(x => x.Authors)
                .Where(expr)
                .OrderBy(propertyToOrder, SortDirection)
                .Skip(skip)
                .Take(pageSize)
                .ToList();
        }

    }
}
