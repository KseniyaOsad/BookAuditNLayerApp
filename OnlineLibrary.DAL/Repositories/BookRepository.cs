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
using System.Threading.Tasks;

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

        public  Task<Book> GetBookByIdAsync(int bookId)
        {
            return  _context.Book
                .Include(b => b.Authors)
                .Include(b => b.Tags)
                .FirstOrDefaultAsync(b => b.Id == bookId);
        }

        public  Task<bool> IsBookIdExistsAsync(int bookId)
        {
            return  _context.Book.AnyAsync(b => b.Id == bookId);
        }

        public Task<List<Book>> GetAllBooksAsync(int skip, int pageSize)
        {
            return _context.Book
                    .Include(b => b.Tags)
                    .Include(b => b.Authors)
                    .Skip(skip)
                    .Take(pageSize)
                    .ToListAsync();
        }

        public Task<int> GetAllBooksCountAsync()
        {
            return _context.Book.CountAsync();
        }

        public  Task<int> GetAllBooksCountAsync(Expression<Func<Book, bool>> expr)
        {
            return  _context.Book
                .Where(expr)
                .CountAsync();
        }

        public  Task<List<Book>> FilterBooksAsync(Expression<Func<Book, bool>> expr, int skip, int pageSize, string propertyToOrder , ListSortDirection SortDirection )
        {
            return  _context.Book
                .Include(x => x.Tags)
                .Include(x => x.Authors)
                .Where(expr)
                .OrderBy(propertyToOrder, SortDirection)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();
        }

    }
}
