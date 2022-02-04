using OnlineLibrary.DAL.Interfaces;
using System;
using System.Threading.Tasks;

namespace OnlineLibrary.DAL.Repositories.Dapper
{
    public class DapperUnitOfWork : IUnitOfWork
    {
        private string _connectionString;

        private Lazy<BookDapperRepository> _bookRepository;

        private Lazy<AuthorDapperRepository> _authorRepository;

        private Lazy<TagDapperRepository> _tagRepository;

        public DapperUnitOfWork(string connectionString)
        {
            _connectionString = connectionString;
            _bookRepository = new Lazy<BookDapperRepository>(() => new BookDapperRepository(_connectionString));
            _authorRepository = new Lazy<AuthorDapperRepository>(() => new AuthorDapperRepository(_connectionString));
            _tagRepository = new Lazy<TagDapperRepository>(() => new TagDapperRepository(_connectionString));
        }

        public IBookRepository BookRepository
        {
            get
            {
                return _bookRepository.Value;
            }
        }

        public IAuthorRepository AuthorRepository
        {
            get
            {
                return _authorRepository.Value;
            }
        }

        public ITagRepository TagRepository
        {
            get
            {
                return _tagRepository.Value;
            }
        }

        public void Dispose()
        {
            // Do nothing.
        }

        public async Task SaveAsync()
        {
        }
    }
}
