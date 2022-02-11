using System.Threading.Tasks;

namespace OnlineLibrary.BLL.Interfaces
{
    public interface IDataExportService
    {
        Task<string> GetAllBooksAsync();

        Task<string> GetAllReservationsAsync();

        Task<string> GetBookReservationsAsync(int bookId);

        Task<string> GetUserReservationsAsync(int userId);
    }
}
