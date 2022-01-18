using OnlineLibrary.Common.Entities;
using OnlineLibrary.DAL.EF;
using OnlineLibrary.DAL.Interfaces;
using System;

namespace OnlineLibrary.DAL.Repositories
{
    public class EFUnitOfWork : IUnitOfWork
    {
        private BookContext _db;
        private Lazy<BookRepository> _bookRepository;
        private Lazy<AuthorRepository> _authorRepository;
        private bool _disposed = false;

        public EFUnitOfWork(BookContext context)
        {
            _db = context;
            _bookRepository = new Lazy<BookRepository>(() => new BookRepository(_db));
            _authorRepository = new Lazy<AuthorRepository>(() => new AuthorRepository(_db));
        }

        public IBookRepository<Book> Book
        {
            get
            {
                return _bookRepository.Value;
            }
        }

        public IAuthorRepository<Author> Author
        {
            get
            {
                return _authorRepository.Value;
            }
        }

        public virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _db.Dispose();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
