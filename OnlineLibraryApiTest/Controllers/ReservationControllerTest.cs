using AutoMapper;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OnlineLibrary.API.Controllers;
using OnlineLibrary.API.Model;
using OnlineLibrary.API.Validator;
using OnlineLibrary.BLL.Interfaces;
using OnlineLibrary.Common.DBEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineLibraryApiTest.Controllers
{
    [TestClass]
    public class ReservationControllerTest
    {
        private ReservationController _reservationController;

        private Mock<IReservationService> _mockReservationService = new Mock<IReservationService>();

        private ReservationModelValidator _reservationModelValidator = new ReservationModelValidator();

        private Mock<ILogger<ReservationController>> _mockILogger = new Mock<ILogger<ReservationController>>();

        private Mock<IMapper> _mockMapper = new Mock<IMapper>();

        // Task<IActionResult> CreateAsync([FromBody] ReservationModel reservationModel)
       
        [TestMethod]
        public async Task Create_Reservation_Ok()
        {
            _mockMapper.Setup(x => x.Map<ReservationModel, Reservation>(It.IsAny<ReservationModel>())).Returns(new Reservation());
            _mockReservationService.Setup(x => x.CreateReservationAsync(It.IsAny<Reservation>())).Returns(Task.FromResult(1));
            _reservationController = new ReservationController(_mockReservationService.Object, _mockMapper.Object, _mockILogger.Object);

            var result = await _reservationController.CreateAsync(new ReservationModel());
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            _mockMapper.Verify(x => x.Map<ReservationModel, Reservation>(It.IsAny<ReservationModel>()), Times.Once);
            _mockReservationService.Verify(x => x.CreateReservationAsync(It.IsAny<Reservation>()), Times.Once);
        }

        // Task<IActionResult> CloseReserveAsync([FromBody] ReservationModel reservationModel)
       
        [TestMethod]
        public async Task Close_Reservation_Ok()
        {
            _mockMapper.Setup(x => x.Map<ReservationModel, Reservation>(It.IsAny<ReservationModel>())).Returns(new Reservation());
            _mockReservationService.Setup(x => x.CloseReservationAsync(It.IsAny<Reservation>())).Returns(Task.FromResult(1));
            _reservationController = new ReservationController(_mockReservationService.Object, _mockMapper.Object, _mockILogger.Object);

            var result = await _reservationController.CloseReserveAsync(new ReservationModel());
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            _mockMapper.Verify(x => x.Map<ReservationModel, Reservation>(It.IsAny<ReservationModel>()), Times.Once);
            _mockReservationService.Verify(x => x.CloseReservationAsync(It.IsAny<Reservation>()), Times.Once);
        }

        //Task<IActionResult> GetAllReservationsAsync()

        [TestMethod]
        public async Task Get_AllUsers_Ok()
        {
            _mockReservationService.Setup(x => x.GetAllReservationsAsync()).Returns(Task.FromResult(new List<Reservation>()));
            _reservationController = new ReservationController(_mockReservationService.Object, _mockMapper.Object, _mockILogger.Object);
            
            var result = await _reservationController.GetAllReservationsAsync();
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            _mockReservationService.Verify(x => x.GetAllReservationsAsync(), Times.Once);
        }

        // Validate ReservationModel - it passed as [FromBody] in CreateAsync and CloseReserveAsync methods

        [TestMethod]
        [DataRow(-1, -1)]
        [DataRow(0, 0)]
        public void Validate_CreateBook_FieldIsIncorrect( int bId, int uId)
        {
            ReservationModel reservationModel = new ReservationModel() { BookId = bId, UserId = uId };
            var result = _reservationModelValidator.TestValidate(reservationModel);
            result.ShouldHaveValidationErrorFor(x => x.BookId);
            result.ShouldHaveValidationErrorFor(x => x.UserId);
        }
    }
}
