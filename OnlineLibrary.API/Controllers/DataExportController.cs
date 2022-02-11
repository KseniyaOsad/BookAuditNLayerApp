using OnlineLibrary.BLL.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using OnlineLibrary.API.Filters;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text;

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

        private async Task<FileStreamResult> GetFile(string textToWrite)
        {
            MemoryStream memoryStream = new MemoryStream();
            StreamWriter writer = new StreamWriter(memoryStream);
            await writer.WriteLineAsync(textToWrite);
            await writer.FlushAsync();
            memoryStream.Seek(0, SeekOrigin.Begin);
            return new FileStreamResult(memoryStream, "application/csv")
            {
                FileDownloadName = "File.csv"
            };
        }

        // GET: api/data-exports/books
        [HttpGet("books")]
        public async Task<IActionResult> GetAllBooksAsync()
        {
            string text = await _dataExport.GetAllBooksAsync();
            _logger.LogInformation($"Get books info as string.");
            return await GetFile(text);
        }

        // GET: api/data-exports/reservations
        [HttpGet("reservations")]
        public async Task<IActionResult> GetAllReservationsAsync()
        {
            string text = await _dataExport.GetAllReservationsAsync();
            _logger.LogInformation($"Get reservations info as string.");
            return await GetFile(text);
        }

        // GET: api/data-exports/reservations/book/{id}
        [HttpGet("reservations/book/{id}")]
        public async Task<IActionResult> GetBookReservationsAsync(int id)
        {
            string text =  await _dataExport.GetBookReservationsAsync(id);
            _logger.LogInformation($"Get book reservations info as string. Book id = {id}");
            return await GetFile(text);
        }

        // GET: api/data-exports/reservations/user/{id}
        [HttpGet("reservations/user/{id}")]
        public async Task<IActionResult> GetUserReservationsAsync(int id)
        {
            string text = await _dataExport.GetUserReservationsAsync(id);
            _logger.LogInformation($"Get  user reservations info as string. User id = {id}");
            return await GetFile(text);
        }
    }
}
