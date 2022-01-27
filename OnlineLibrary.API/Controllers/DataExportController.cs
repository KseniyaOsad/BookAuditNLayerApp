using OnlineLibrary.BLL.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using OnlineLibrary.Common.Filters;
using System.Threading.Tasks;

namespace OnlineLibrary.API.Controllers
{
    [Route("api/data-exports")]
    [ApiController]
    [TypeFilter(typeof(GenericExceptionFilter))]
    public class DataExportController : ControllerBase
    {
        private readonly IDataExportService _dataExport;

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
            return new FileContentResult(await System.IO.File.ReadAllBytesAsync(_path + _fileName), "application/csv")
            {
                FileDownloadName = _fileName
            };
        }
    }
}
