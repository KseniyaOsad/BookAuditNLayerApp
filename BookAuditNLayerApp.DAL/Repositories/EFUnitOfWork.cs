using BookAuditNLayer.GeneralClassLibrary.Entities;
using BookAuditNLayerApp.DAL.EF;
using BookAuditNLayerApp.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookAuditNLayerApp.DAL.Repositories
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
        }

        public IBookRepository<Book> Book
        {
            get
            {
                if (_bookRepository == null)
                    _bookRepository = new Lazy<BookRepository>(()=> new BookRepository(_db));
                return _bookRepository.Value;
            }
        }

        public IAuthorRepository<Author> Author
        {
            get
            {
                if (_authorRepository == null)
                    _authorRepository = new Lazy<AuthorRepository>(()=>new AuthorRepository(_db));
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
            GC.SuppressFinalize(this); // I need it?
        }
    }
}
