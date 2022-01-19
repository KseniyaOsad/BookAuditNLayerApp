﻿using OnlineLibrary.BLL.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;

namespace OnlineLibrary.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DataExportController : Controller
    {
        private readonly IDataExportService _dataExport;

        private readonly string _path;

        private const string _fileName = "book.csv";

        public DataExportController(IWebHostEnvironment hostEnvironment, IDataExportService iData)
        {
            _dataExport = iData;
            _path = hostEnvironment.ContentRootPath + @"\Data\csvFiles\";
        }

        // GET: api/DataExport/GetFile
        [HttpGet]
        public IActionResult GetFile()
        {
            try
            {
                _dataExport.WriteCsv(_path, _fileName);
                return new FileContentResult(System.IO.File.ReadAllBytes(_path + _fileName), "application/csv")
                {
                    FileDownloadName = _fileName
                };
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }
    }
}