using OnlineLibrary.BLL.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using OnlineLibrary.API.Filters;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace OnlineLibrary.API.Controllers
{
    [Route("api/data-exports")]
    [ApiController]
    [TypeFilter(typeof(GenericExceptionFilter))]
    public class DataExportController : ControllerBase
    {
        private readonly IDataExportService _dataExport;

        private readonly ILogger<DataExportController> _logger;

        private readonly string _path;

        private const string _fileName = "book.csv";

        public DataExportController(IWebHostEnvironment hostEnvironment, IDataExportService iData, ILogger<DataExportController> logger)
        {
            _dataExport = iData;
            _path = hostEnvironment.ContentRootPath + @"\Data\csvFiles\";
            _logger = logger;
        }

        // GET: api/data-exports
        [HttpGet]
        public async Task<IActionResult> GetFileAsync()
        {
            await _dataExport.WriteCsvAsync(_path, _fileName);
            _logger.LogInformation($"Writting data to csw file, {_fileName}.");
            FileContentResult fileResult = new FileContentResult(await System.IO.File.ReadAllBytesAsync(_path + _fileName), "application/csv")
            {
                FileDownloadName = _fileName
            };
            _logger.LogInformation($"Read all bytes from csw file, {_fileName}.");
            return fileResult;
        }
    }
}
