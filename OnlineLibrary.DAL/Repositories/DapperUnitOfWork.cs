using OnlineLibrary.DAL.Interfaces;
using System;

namespace OnlineLibrary.DAL.Repositories.Dapper
{
    public class DapperUnitOfWork : IUnitOfWork
    {
        private string _connectionString;

        private Lazy<BookRepository> _bookRepository;

        private Lazy<AuthorRepository> _authorRepository;

        private Lazy<TagRepository> _tagRepository;

        private Lazy<UserRepository> _userRepository;

        private Lazy<ReservationRepository> _reservationRepository;

        public DapperUnitOfWork(string connectionString)
        {
            _connectionString = connectionString;
            _bookRepository = new Lazy<BookRepository>(() => new BookRepository(_connectionString));
            _authorRepository = new Lazy<AuthorRepository>(() => new AuthorRepository(_connectionString));
            _tagRepository = new Lazy<TagRepository>(() => new TagRepository(_connectionString));
            _userRepository = new Lazy<UserRepository>(() => new UserRepository(_connectionString));
            _reservationRepository = new Lazy<ReservationRepository>(() => new ReservationRepository(_connectionString));
        }

        public IBookRepository BookRepository
        {
            get =>  _bookRepository.Value;
        }

        public IAuthorRepository AuthorRepository
        {
            get => _authorRepository.Value;
        }

        public ITagRepository TagRepository
        {
            get => _tagRepository.Value;
        }

        public IUserRepository UserRepository
        {
            get => _userRepository.Value;
        }

        public IReservationRepository ReservationRepository
        {
            get => _reservationRepository.Value;
        }

    }
}
