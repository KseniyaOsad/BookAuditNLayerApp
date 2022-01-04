using BookAuditNLayer.GeneralClassLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookAuditNLayerApp.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IBookRepository<Book> Book { get; }
        IAuthorRepository<Author> Author { get; }
    }
}
