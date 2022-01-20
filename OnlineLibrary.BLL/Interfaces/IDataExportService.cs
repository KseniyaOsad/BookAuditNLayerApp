namespace OnlineLibrary.BLL.Interfaces
{
    public interface IDataExportService
    {
        void WriteCsv(string path, string filename);
    }
}
