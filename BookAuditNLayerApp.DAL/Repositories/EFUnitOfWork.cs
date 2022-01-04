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
        private BookRepository _bookRepository;
        private AuthorRepository _authorRepository;
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
                    _bookRepository = new BookRepository(_db);
                return _bookRepository;
            }
        }

        public IAuthorRepository<Author> Author
        {
            get
            {
                if (_authorRepository == null)
                    _authorRepository = new AuthorRepository(_db);
                return _authorRepository;
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
            GC.SuppressFinalize(this);
        }
    }
}
