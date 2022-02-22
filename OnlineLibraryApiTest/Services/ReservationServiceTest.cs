using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OnlineLibrary.BLL.Services;
using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.Common.Exceptions;
using OnlineLibrary.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineLibraryApiTest.Services
{
    [TestClass]
    public class ReservationServiceTest
    {
        private ReservationService _reservationService;

        private Mock<IUnitOfWork> _mockUnitOfWork = new Mock<IUnitOfWork>();

        private Mock<IReservationRepository> _mockReservationRepository = new Mock<IReservationRepository>();

        [TestInitialize]
        public void InitializeTest()
        {
            _mockUnitOfWork.Setup(x => x.ReservationRepository).Returns(_mockReservationRepository.Object);
        }

        // Task CloseReservationAsync(Reservation reservation)

        [TestMethod]
        public async Task Close_Reservation_Ok()
        {
            _mockUnitOfWork.Setup(x => x.ReservationRepository.GetBookReservationLastRow(It.IsAny<int>())).Returns(Task.FromResult(new Reservation() { Id = 1, UserId = 1, ReturnDate = null }));
            _mockUnitOfWork.Setup(x => x.ReservationRepository.CloseReservationAsync(It.IsAny<Reservation>()));
            _reservationService = new ReservationService(_mockUnitOfWork.Object);
            await _reservationService.CloseReservationAsync(new Reservation() { Book = new Book() { Id = 1 }, User = new User() { Id = 1} });
            _mockUnitOfWork.Verify(x => x.ReservationRepository.CloseReservationAsync(It.IsAny<Reservation>()), Times.Once);
        }

        [TestMethod]
        public async Task Close_Reservation_BookIsNotInReserve()
        {
            _mockUnitOfWork.Setup(x => x.ReservationRepository.GetBookReservationLastRow(It.IsAny<int>())).Returns(Task.FromResult(new Reservation() { Id = 1, UserId = 1 , ReturnDate = new DateTime() }));
            _reservationService = new ReservationService(_mockUnitOfWork.Object);
            await Assert.ThrowsExceptionAsync<OLBadRequest>(() => _reservationService.CloseReservationAsync(new Reservation() { Book = new Book() { Id = 1 }, User = new User() { Id = 1 } }));
            
            _mockUnitOfWork.Verify(x => x.ReservationRepository.CloseReservationAsync(It.IsAny<Reservation>()), Times.Never);
        }

        // Task<int> CreateReservationAsync(Reservation reservation)

        [TestMethod]
        public async Task Create_Reservation_Ok()
        {
            _mockUnitOfWork.Setup(x => x.UserRepository.IsUserExistAsync(It.IsAny<int>())).Returns(Task.FromResult(true));
            _mockUnitOfWork.Setup(x => x.BookRepository.GetBookByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(new Book() { InArchive = false }));
            _mockUnitOfWork.Setup(x => x.ReservationRepository.CreateReservationAsync(It.IsAny<Reservation>()));
            _reservationService = new ReservationService(_mockUnitOfWork.Object);

            await _reservationService.CreateReservationAsync(new Reservation() { Id = 1, User = new User() { Id = 1 }, Book = new Book() { Id = 1 } });
            _mockUnitOfWork.Verify(x => x.ReservationRepository.CreateReservationAsync(It.IsAny<Reservation>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.UserRepository.IsUserExistAsync(It.IsAny<int>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.BookRepository.GetBookByIdAsync(It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        public async Task Create_Reservation_UserDontExist()
        {
            _mockUnitOfWork.Setup(x => x.UserRepository.IsUserExistAsync(It.IsAny<int>())).Returns(Task.FromResult(false));
            _mockUnitOfWork.Setup(x => x.BookRepository.GetBookByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(new Book() { InArchive = false }));
            _mockUnitOfWork.Setup(x => x.ReservationRepository.CreateReservationAsync(It.IsAny<Reservation>()));
            _reservationService = new ReservationService(_mockUnitOfWork.Object);

            await Assert.ThrowsExceptionAsync<OLBadRequest>(() => _reservationService.CreateReservationAsync(new Reservation() { Id = 1, User = new User() { Id = 1 }, Book = new Book() { Id = 1 } }));
            _mockUnitOfWork.Verify(x => x.ReservationRepository.CreateReservationAsync(It.IsAny<Reservation>()), Times.Never);
            _mockUnitOfWork.Verify(x => x.UserRepository.IsUserExistAsync(It.IsAny<int>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.BookRepository.GetBookByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public async Task Create_Reservation_BookNotFound()
        {
            _mockUnitOfWork.Setup(x => x.UserRepository.IsUserExistAsync(It.IsAny<int>())).Returns(Task.FromResult(true));
            Book book = null;
            _mockUnitOfWork.Setup(x => x.BookRepository.GetBookByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(book));
            _mockUnitOfWork.Setup(x => x.ReservationRepository.CreateReservationAsync(It.IsAny<Reservation>()));
            _reservationService = new ReservationService(_mockUnitOfWork.Object);

            await Assert.ThrowsExceptionAsync<OLNotFound>(() => _reservationService.CreateReservationAsync(new Reservation() { Id = 1, User = new User() { Id = 1 }, Book = new Book() { Id = 1 } }));

            _mockUnitOfWork.Verify(x => x.ReservationRepository.CreateReservationAsync(It.IsAny<Reservation>()), Times.Never);
            _mockUnitOfWork.Verify(x => x.UserRepository.IsUserExistAsync(It.IsAny<int>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.BookRepository.GetBookByIdAsync(It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        public async Task Create_Reservation_BookInArchive()
        {
            _mockUnitOfWork.Setup(x => x.UserRepository.IsUserExistAsync(It.IsAny<int>())).Returns(Task.FromResult(true));
            _mockUnitOfWork.Setup(x => x.BookRepository.GetBookByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(new Book() { InArchive = true }));
            _mockUnitOfWork.Setup(x => x.ReservationRepository.CreateReservationAsync(It.IsAny<Reservation>()));
            _reservationService = new ReservationService(_mockUnitOfWork.Object);
           
            await Assert.ThrowsExceptionAsync<OLBadRequest>(() => _reservationService.CreateReservationAsync(new Reservation() { Id = 1, User = new User() { Id = 1 }, Book = new Book() { Id = 1 } }));
            _mockUnitOfWork.Verify(x => x.ReservationRepository.CreateReservationAsync(It.IsAny<Reservation>()), Times.Never);
            _mockUnitOfWork.Verify(x => x.UserRepository.IsUserExistAsync(It.IsAny<int>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.BookRepository.GetBookByIdAsync(It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        public async Task Create_Reservation_OLInternalServerError()
        {
            _mockUnitOfWork.Setup(x => x.UserRepository.IsUserExistAsync(It.IsAny<int>())).Returns(Task.FromResult(true));
            _mockUnitOfWork.Setup(x => x.BookRepository.GetBookByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(new Book() { InArchive = false }));
            _mockUnitOfWork.Setup(x => x.ReservationRepository.CreateReservationAsync(It.IsAny<Reservation>()));
            _reservationService = new ReservationService(_mockUnitOfWork.Object);
             
            await Assert.ThrowsExceptionAsync<OLInternalServerError>(() =>
                _reservationService.CreateReservationAsync(new Reservation() { Id = 0, User = new User() { Id = 1 }, Book = new Book() { Id = 1 } })
            );
            _mockUnitOfWork.Verify(x => x.ReservationRepository.CreateReservationAsync(It.IsAny<Reservation>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.UserRepository.IsUserExistAsync(It.IsAny<int>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.BookRepository.GetBookByIdAsync(It.IsAny<int>()), Times.Once);
        }

        // Task<List<Reservation>> GetAllReservationsAsync()

        [TestMethod]
        public async Task Get_AllReservations_Ok()
        {
            _mockUnitOfWork.Setup(x => x.ReservationRepository.GetAllReservationsAsync()).Returns(Task.FromResult(new List<Reservation>()));
            _reservationService = new ReservationService(_mockUnitOfWork.Object);
            await _reservationService.GetAllReservationsAsync();
            _mockUnitOfWork.Verify(x => x.ReservationRepository.GetAllReservationsAsync(), Times.Once);
        }
    }
}
