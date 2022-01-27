using System.Threading.Tasks;

namespace OnlineLibrary.BLL.Interfaces
{
    public interface IDataExportService
    {
        Task WriteCsvAsync(string path, string filename);
    }
}
