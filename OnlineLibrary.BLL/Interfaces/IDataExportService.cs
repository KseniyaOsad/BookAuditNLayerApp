using System.Threading.Tasks;

namespace OnlineLibrary.BLL.Interfaces
{
    public interface IDataExportService
    {
        Task WriteBooksToCsvAsync(string path, string filename);

        Task WriteReservationsToCsvAsync(string path, string filename);

        Task WriteBookReservationsToCsvAsync(string path, string filename, int bookId);

        Task WriteUserReservationsToCsvAsync(string path, string filename, int userId);
    }
}
