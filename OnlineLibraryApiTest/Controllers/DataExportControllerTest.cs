using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OnlineLibrary.API.Controllers;
using OnlineLibrary.BLL.Interfaces;
using System;

namespace OnlineLibraryApiTest.Controllers
{

    [TestClass]
    public class DataExportControllerTest
    {
        private DataExportController _dataExportController;

        private Mock<IDataExportService> _mockDataExportService = new Mock<IDataExportService>();

        private Mock<IWebHostEnvironment> _mockHostingEnvironment = new Mock<IWebHostEnvironment>();

        [TestMethod]
        public void Get_File_OK()
        {
            _mockHostingEnvironment.Setup(x => x.ContentRootPath).Returns(@"C:\Users\theks\Desktop\C\OnlineLibrary\OnlineLibrary.API");
            _dataExportController = new DataExportController(_mockHostingEnvironment.Object, _mockDataExportService.Object);
            var result = _dataExportController.GetFile();
            var okResult = result as FileContentResult;

            Assert.IsNotNull(okResult);
            _mockDataExportService.Verify(x => x.WriteCsv(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}
