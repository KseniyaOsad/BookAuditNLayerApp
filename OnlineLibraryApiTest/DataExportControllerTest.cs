using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OnlineLibrary.API.Controllers;
using OnlineLibrary.BLL.Interfaces;

namespace OnlineLibraryApiTest
{

    [TestClass]
    public class DataExportControllerTest
    {
        private DataExportController dataExportController;

        private Mock<IDataExportService> mockDataExportService = new Mock<IDataExportService>();

        private Mock<IWebHostEnvironment> mockHostingEnvironment = new Mock<IWebHostEnvironment>();

        [TestMethod]
        public void Get_File_ListIsEmpty()
        {
            dataExportController = new DataExportController(mockHostingEnvironment.Object, mockDataExportService.Object);

            var result = dataExportController.GetFile();
            var badResult = result as NotFoundObjectResult;

            Assert.IsNotNull(badResult);
            Assert.AreEqual(404, badResult.StatusCode);
        }

        [TestMethod]
        public void Get_File_OK()
        {
            mockHostingEnvironment.Setup(x => x.ContentRootPath).Returns(@"C:\Users\theks\Desktop\C\OnlineLibrary\OnlineLibrary.API");
            dataExportController = new DataExportController(mockHostingEnvironment.Object, mockDataExportService.Object);
            var result = dataExportController.GetFile();
            var okResult = result as FileContentResult;

            Assert.IsNotNull(okResult);
            mockDataExportService.Verify(x => x.WriteCsv(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}
