using OnlineLibrary.BLL.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using OnlineLibrary.Common.Filters;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using log4net;

namespace OnlineLibrary.API.Controllers
{
    [Route("api/data-exports")]
    [ApiController]
    [TypeFilter(typeof(GenericExceptionFilter))]
    public class DataExportController : ControllerBase
    {
        private readonly IDataExportService _dataExport;

        private static readonly ILog _logger = LogManager.GetLogger(typeof(DataExportController));

        private readonly string _path;

        private const string _fileName = "book.csv";

        public DataExportController(IWebHostEnvironment hostEnvironment, IDataExportService iData)
        {
            _dataExport = iData;
            _path = hostEnvironment.ContentRootPath + @"\Data\csvFiles\";
        }

        // GET: api/data-exports
        [HttpGet]
        public async Task<IActionResult> GetFileAsync()
        {
            await _dataExport.WriteCsvAsync(_path, _fileName);
            _logger.Info($"Writting data to csw file, {_fileName}.");
            FileContentResult fileResult = new FileContentResult(await System.IO.File.ReadAllBytesAsync(_path + _fileName), "application/csv")
            {
                FileDownloadName = _fileName
            };
            _logger.Info($"Read all bytes from csw file, {_fileName}.");
            return fileResult;
        }
    }
}
