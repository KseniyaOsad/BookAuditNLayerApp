namespace OnlineLibrary.DAL.Interfaces
{
    public interface IUnitOfWork
    {
        IBookRepository BookRepository { get; }

        IAuthorRepository AuthorRepository { get; }

        ITagRepository TagRepository { get; }

        IUserRepository UserRepository { get; }

        IReservationRepository ReservationRepository { get; }
    }
}
