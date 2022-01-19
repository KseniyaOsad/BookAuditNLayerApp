using OnlineLibrary.Common.Entities;
using System;

namespace OnlineLibrary.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        void Save();

        IBookRepository BookRepository { get; }

        IAuthorRepository AuthorRepository { get; }
    }
}
