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

        private async Task WriteCsvAsync(string path, string filename, string text)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(path + filename, false))
                {
                    await sw.WriteAsync(text);
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                throw new OLInternalServerError("Failed to write file: " + ex.ToString());
            }
        }

        public async Task WriteBooksToCsvAsync(string path, string filename)
        {
            ExceptionExtensions.Check<OLInternalServerError>(string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(filename), "File path is empty");

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

            await WriteCsvAsync(path, filename, allTextToWrite.ToString());
        }

        public async Task WriteReservationsToCsvAsync(string path, string filename)
        {
            ExceptionExtensions.Check<OLInternalServerError>(string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(filename), "File path is empty");
            List<Reservation> reservations = await unitOfWork.ReservationRepository.GetAllReservationsAsync();
            ExceptionExtensions.Check<OLNotFound>(!reservations.Any(), "There are no reservations");
            await WriteReservations(path, filename, reservations);
        }

        public async Task WriteBookReservationsToCsvAsync(string path, string filename, int bookId)
        {
            ExceptionExtensions.Check<OLInternalServerError>(string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(filename), "File path is empty");
            List<Reservation> reservations = await unitOfWork.ReservationRepository.GetBookReservationHistoryAsync(bookId);
            ExceptionExtensions.Check<OLNotFound>(!reservations.Any(), $"This book has no reservation history. Book id = {bookId}");
            await WriteReservations(path, filename, reservations);
        }

        public async Task WriteUserReservationsToCsvAsync(string path, string filename, int userId)
        {
            ExceptionExtensions.Check<OLInternalServerError>(string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(filename), "File path is empty");
            List<Reservation> reservations = await unitOfWork.ReservationRepository.GetUserReservationHistoryAsync(userId);
            ExceptionExtensions.Check<OLNotFound>(!reservations.Any(), $"This user has no reservation history. User id = {userId}");
            await WriteReservations(path, filename, reservations);
        }

        private async Task WriteReservations(string path, string filename, List<Reservation> reservations)
        {
            StringBuilder allTextToWrite = new StringBuilder();
            reservations.Select(
                    r => allTextToWrite.Append(
                        r.ToString())
                    ).ToList();
            await WriteCsvAsync(path, filename, allTextToWrite.ToString());
        }
    }
}
