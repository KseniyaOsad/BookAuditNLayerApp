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

        private Lazy<TagRepository> _tagRepository;

        public EFUnitOfWork(BookContext context)
        {
            _db = context;
            _bookRepository = new Lazy<BookRepository>(() => new BookRepository(_db));
            _authorRepository = new Lazy<AuthorRepository>(() => new AuthorRepository(_db));
            _tagRepository = new Lazy<TagRepository>(() => new TagRepository(_db));
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
            _db.Dispose();
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
