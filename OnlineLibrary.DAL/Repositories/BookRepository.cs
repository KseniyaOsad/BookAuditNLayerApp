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

        public async Task<Book> GetBookByIdAsync(int bookId)
        {
            return await _context.Book
                .Include(b => b.Authors)
                .Include(b => b.Tags)
                .FirstOrDefaultAsync(b => b.Id == bookId);
        }

        public async Task<bool> IsBookIdExistsAsync(int bookId)
        {
            return await _context.Book.AnyAsync(b => b.Id == bookId);
        }

        public async Task<List<Book>> GetAllBooksAsync(int skip, int pageSize)
        {
            return await _context.Book
                    .Include(b => b.Tags)
                    .Include(b => b.Authors)
                    .Skip(skip)
                    .Take(pageSize)
                    .ToListAsync();
        }

        public async Task<int> GetAllBooksCountAsync()
        {
            return await _context.Book.CountAsync();
        }

        public async Task<int> GetAllBooksCountAsync(Expression<Func<Book, bool>> expr)
        {
            return await _context.Book
                .Where(expr)
                .CountAsync();
        }

        public async Task<List<Book>> FilterBooksAsync(Expression<Func<Book, bool>> expr, int skip, int pageSize, string propertyToOrder , ListSortDirection SortDirection )
        {
            return await _context.Book
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
