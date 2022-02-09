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

        public DataExportController(IWebHostEnvironment hostEnvironment, IDataExportService iData, ILogger<DataExportController> logger)
        {
            _dataExport = iData;
            _path = hostEnvironment.ContentRootPath + @"\Data\csvFiles\";
            _logger = logger;
        }

        private async Task<FileContentResult> GetFile(string fileName)
        {
            FileContentResult fileResult = new FileContentResult(await System.IO.File.ReadAllBytesAsync(_path + fileName), "application/csv")
            {
                FileDownloadName = fileName
            };
            _logger.LogInformation($"Read all bytes from csw file, {fileName}.");
            return fileResult;
        }

        // GET: api/data-exports/books
        [HttpGet("books")]
        public async Task<IActionResult> GetAllBooksAsync()
        {
            const string fileName = "book.csv";
            await _dataExport.WriteBooksToCsvAsync(_path, fileName);
            _logger.LogInformation($"Writting books info to csw file, {fileName}.");
            return await GetFile(fileName);
        }

        // GET: api/data-exports/reservations
        [HttpGet("reservations")]
        public async Task<IActionResult> GetAllReservationsAsync()
        {
            const string fileName = "reservations.csv";
            await _dataExport.WriteReservationsToCsvAsync(_path, fileName);
            _logger.LogInformation($"Writting reservations info to csw file, {fileName}.");
            return await GetFile(fileName);
        }

        // GET: api/data-exports/reservations/book/{id}
        [HttpGet("reservations/book/{id}")]
        public async Task<IActionResult> GetBookReservationsAsync(int id)
        {
            const string fileName = "reservations.csv";
            await _dataExport.WriteBookReservationsToCsvAsync(_path, fileName, id);
            _logger.LogInformation($"Writting book reservations to csw file, {fileName}. Book id = {id}");
            return await GetFile(fileName);
        }

        // GET: api/data-exports/reservations/user/{id}
        [HttpGet("reservations/user/{id}")]
        public async Task<IActionResult> GetUserReservationsAsync(int id)
        {
            const string fileName = "reservations.csv";
            await _dataExport.WriteUserReservationsToCsvAsync(_path, fileName, id);
            _logger.LogInformation($"Writting user reservations to csw file, {fileName}. User id = {id}");
            return await GetFile(fileName);
        }
    }
}
