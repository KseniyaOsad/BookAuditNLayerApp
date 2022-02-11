using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OnlineLibrary.API.Controllers;
using OnlineLibrary.BLL.Interfaces;
using System.Threading.Tasks;

namespace OnlineLibraryApiTest.Controllers
{

    [TestClass]
    public class DataExportControllerTest
    {
        private DataExportController _dataExportController;

        private Mock<IDataExportService> _mockDataExportService = new Mock<IDataExportService>();

        private Mock<IWebHostEnvironment> _mockHostingEnvironment = new Mock<IWebHostEnvironment>();
        
        private Mock<ILogger<DataExportController>> _mockILogger = new Mock<ILogger<DataExportController>>();

        // Task<IActionResult> GetAllBooksAsync()

        [TestMethod]
        public async Task Get_AllBooks_File_OK()
        {
            _dataExportController = new DataExportController(_mockHostingEnvironment.Object, _mockDataExportService.Object, _mockILogger.Object);
            var result = await _dataExportController.GetAllBooksAsync();
            var okResult = result as FileStreamResult;

            Assert.IsNotNull(okResult);
            _mockDataExportService.Verify(x => x.GetAllBooksAsync(), Times.Once);
        }

        // Task<IActionResult> GetAllReservationsAsync()

        [TestMethod]
        public async Task Get_AllReservations_File_OK()
        {
            _dataExportController = new DataExportController(_mockHostingEnvironment.Object, _mockDataExportService.Object, _mockILogger.Object);
            var result = await _dataExportController.GetAllReservationsAsync();
            var okResult = result as FileStreamResult;

            Assert.IsNotNull(okResult);
            _mockDataExportService.Verify(x => x.GetAllReservationsAsync(), Times.Once);
        }

        // Task<IActionResult> GetBookReservationsAsync(int id)

        [TestMethod]
        public async Task Get_BookReservations_File_OK()
        {
            _dataExportController = new DataExportController(_mockHostingEnvironment.Object, _mockDataExportService.Object, _mockILogger.Object);
            var result = await _dataExportController.GetBookReservationsAsync(1);
            var okResult = result as FileStreamResult;

            Assert.IsNotNull(okResult);
            _mockDataExportService.Verify(x => x.GetBookReservationsAsync(It.IsAny<int>()), Times.Once);
        }

        // Task<IActionResult> GetUserReservationsAsync(int id)

        [TestMethod]
        public async Task Get_UserReservations_File_OK()
        {
            _dataExportController = new DataExportController(_mockHostingEnvironment.Object, _mockDataExportService.Object, _mockILogger.Object);
            var result = await _dataExportController.GetUserReservationsAsync(1);
            var okResult = result as FileStreamResult;

            Assert.IsNotNull(okResult);
            _mockDataExportService.Verify(x => x.GetUserReservationsAsync(It.IsAny<int>()), Times.Once);
        }
    }
}
