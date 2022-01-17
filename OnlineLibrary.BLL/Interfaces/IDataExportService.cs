using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineLibrary.BLL.Interfaces
{
    public interface IDataExportService
    {
        void WriteCsv(string path, string filename);
    }
}
