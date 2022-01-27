using System;
using System.Threading.Tasks;

namespace OnlineLibrary.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Task SaveAsync();

        IBookRepository BookRepository { get; }

        IAuthorRepository AuthorRepository { get; }

        ITagRepository TagRepository { get; }
    }
}
