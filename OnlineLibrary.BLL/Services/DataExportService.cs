using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.Common.Extensions;
using OnlineLibrary.BLL.Interfaces;
using OnlineLibrary.BLL.Model;
using OnlineLibrary.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OnlineLibrary.Common.Exceptions;
using System.Threading.Tasks;

namespace OnlineLibrary.BLL.Services
{
    public class DataExportService : IDataExportService
    {
        private readonly IUnitOfWork unitOfWork;

        public DataExportService(IUnitOfWork uow)
        {
            unitOfWork = uow;
        }

        public async Task<string> GetAllBooksAsync()
        {
            List<Book> books = await unitOfWork.BookRepository.GetAllBooksForCsvAsync();
            ExceptionExtensions.Check<OLNotFound>(!books.Any(), "Books don't exist");

            StringBuilder allTextToWrite = new StringBuilder();
            books.
                Select(
                    b => allTextToWrite.Append(
                        new BookInfoToCSV()
                        {
                            Id = b.Id,
                            Title = b.Name,
                            AuthorNames = String.Join(" & ", b.Authors.Select(a => a.Name).ToArray()),
                            TagNames = String.Join(" & ", b.Tags.Select(t => t.Name).ToArray())
                        }.ToString())
                    ).ToList();

            return allTextToWrite.ToString();
        }

        public async Task<string> GetAllReservationsAsync()
        {
            List<Reservation> reservations = await unitOfWork.ReservationRepository.GetAllReservationsAsync();
            ExceptionExtensions.Check<OLNotFound>(!reservations.Any(), "There are no reservations");
            return GetReservationsInString(reservations);
        }

        public async Task<string> GetBookReservationsAsync(int bookId)
        {
            List<Reservation> reservations = await unitOfWork.ReservationRepository.GetBookReservationHistoryAsync(bookId);
            ExceptionExtensions.Check<OLNotFound>(!reservations.Any(), $"This book has no reservation history. Book id = {bookId}");
            return GetReservationsInString(reservations);
        }

        public async Task<string> GetUserReservationsAsync(int userId)
        {
            List<Reservation> reservations = await unitOfWork.ReservationRepository.GetUserReservationHistoryAsync(userId);
            ExceptionExtensions.Check<OLNotFound>(!reservations.Any(), $"This user has no reservation history. User id = {userId}");
            return GetReservationsInString(reservations)
                ;
        }

        private string GetReservationsInString(List<Reservation> reservations)
        {
            StringBuilder allTextToWrite = new StringBuilder();
            reservations.Select(
                    r => allTextToWrite.Append(
                        r.ToString())
                    ).ToList();
            return allTextToWrite.ToString();
        }
    }
}
