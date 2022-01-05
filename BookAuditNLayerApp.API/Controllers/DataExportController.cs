﻿using BookAuditNLayer.GeneralClassLibrary.Entities;
using BookAuditNLayerApp.BLL.Infrastructure;
using BookAuditNLayerApp.BLL.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BookAuditNLayerApp.WEB.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DataExportController : Controller
    {
        private readonly IDataExportService _dataExport;

        private readonly string _path;

        private const string _fileName = "book.csv";

        [Obsolete]
        public DataExportController(IHostingEnvironment hostEnvironment, IDataExportService iData)
        {
            _dataExport = iData;
            _path = (hostEnvironment.ContentRootPath + "/Data/csvFiles/").Replace("\\", @"\");
        }

        public FileResult TestDownload()
        {
            HttpContext.Response.ContentType = "application/pdf";
            FileContentResult result = new FileContentResult(System.IO.File.ReadAllBytes("YOUR PATH TO PDF"), "application/pdf")
            {
                FileDownloadName = "test.pdf"
            };

            return result;
        }

        // GET: api/DataExport/GetFile
        [HttpGet]
        public IActionResult GetFile()
        {
            try
            {
                _dataExport.WriteCsv(_path, _fileName);
               return new FileContentResult(System.IO.File.ReadAllBytes(_path+ _fileName), "application/csv")
               {
                   FileDownloadName = _fileName
               };
            }
            catch (ValidationException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }
    }
}
