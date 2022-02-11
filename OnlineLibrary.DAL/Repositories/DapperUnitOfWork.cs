using Microsoft.Extensions.Options;
using OnlineLibrary.Common.Connection;
using OnlineLibrary.DAL.Interfaces;
using System;

namespace OnlineLibrary.DAL.Repositories.Dapper
{
    public class DapperUnitOfWork : IUnitOfWork
    {

        private Lazy<BookRepository> _bookRepository;

        private Lazy<AuthorRepository> _authorRepository;

        private Lazy<TagRepository> _tagRepository;

        private Lazy<UserRepository> _userRepository;

        private Lazy<ReservationRepository> _reservationRepository;

        public DapperUnitOfWork(IOptions<DBConnection> connOptions)
        {
            _bookRepository = new Lazy<BookRepository>(() => new BookRepository(connOptions));
            _authorRepository = new Lazy<AuthorRepository>(() => new AuthorRepository(connOptions));
            _tagRepository = new Lazy<TagRepository>(() => new TagRepository(connOptions));
            _userRepository = new Lazy<UserRepository>(() => new UserRepository(connOptions));
            _reservationRepository = new Lazy<ReservationRepository>(() => new ReservationRepository(connOptions));
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
