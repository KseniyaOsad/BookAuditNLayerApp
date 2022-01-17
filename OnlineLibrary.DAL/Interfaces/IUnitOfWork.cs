using OnlineLibrary.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineLibrary.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        void Save();
        
        IBookRepository<Book> Book { get; }
        
        IAuthorRepository<Author> Author { get; }
    }
}
