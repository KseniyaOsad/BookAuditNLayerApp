using System;
using System.Collections.Generic;
using System.Text;

namespace BookAuditNLayerApp.BLL.Interfaces
{
    public interface IDataExportService
    {
        void WriteCsv(string path, string filename);
    }
}
