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
    public class DataExportServiceTest
    {
        private DataExportService _dataExportService;

        private Mock<IUnitOfWork> _mockUnitOfWork = new Mock<IUnitOfWork>();

        private Mock<IBookRepository> _mockBookRepository = new Mock<IBookRepository>();

        private Mock<IAuthorRepository> _mockAuthorRepository = new Mock<IAuthorRepository>();

        [TestInitialize]
        public void InitializeTest()
        {
            _mockUnitOfWork.Setup(x => x.BookRepository).Returns(_mockBookRepository.Object);
            _mockUnitOfWork.Setup(x => x.AuthorRepository).Returns(_mockAuthorRepository.Object);
        }

        // PathIsINcorrect in different methods

        // Task WriteBooksToCsvAsync(string path, string filename)

        [TestMethod]
        public async Task Write_Books_toCSV_ListIsEmpty()
        {
            _mockUnitOfWork.Setup(x => x.BookRepository.GetAllBooksForCsvAsync()).Returns(Task.FromResult(new List<Book>() { }));
            _dataExportService = new DataExportService(_mockUnitOfWork.Object);

            await Assert.ThrowsExceptionAsync<OLNotFound>(() => _dataExportService.GetAllBooksAsync());
            _mockUnitOfWork.Verify(x => x.BookRepository.GetAllBooksForCsvAsync(), Times.Once);
        }

        [TestMethod]
        public async Task Write_Books_toCSV_OK()
        {
            Book book = new Book() { Id = 1, Name = "test", Authors = new List<Author>() { new Author() { Name = "author" } }, Tags = new List<Tag>() { new Tag() { Name = "Tag" } } };
            _mockUnitOfWork.Setup(x => x.BookRepository.GetAllBooksForCsvAsync()).Returns(Task.FromResult(new List<Book>() { book }));
            _dataExportService = new DataExportService(_mockUnitOfWork.Object);

            await _dataExportService.GetAllBooksAsync();
            _mockUnitOfWork.Verify(x => x.BookRepository.GetAllBooksForCsvAsync(), Times.Once);
        }

        // Task WriteReservationsToCsvAsync(string path, string filename)

        [TestMethod]
        public async Task Write_Reservations_toCSV_ListIsEmpty()
        {
            _mockUnitOfWork.Setup(x => x.ReservationRepository.GetAllReservationsAsync()).Returns(Task.FromResult(new List<Reservation>() { }));
            _dataExportService = new DataExportService(_mockUnitOfWork.Object);

            await Assert.ThrowsExceptionAsync<OLNotFound>(() => _dataExportService.GetAllReservationsAsync());
            _mockUnitOfWork.Verify(x => x.ReservationRepository.GetAllReservationsAsync(), Times.Once);
        }

        [TestMethod]
        public async Task Write_Reservations_toCSV_Ok()
        {
            Reservation reservation = new Reservation() { Id = 1, Book = new Book() { Id = 1, Name = "test" }, User = new User() { Id = 1, Name = "test" }, ReservationDate = DateTime.Now };
            _mockUnitOfWork.Setup(x => x.ReservationRepository.GetAllReservationsAsync()).Returns(Task.FromResult(new List<Reservation>() { reservation }));
            _dataExportService = new DataExportService(_mockUnitOfWork.Object);

            await _dataExportService.GetAllReservationsAsync();
            _mockUnitOfWork.Verify(x => x.ReservationRepository.GetAllReservationsAsync(), Times.Once);
        }

        // Task WriteBookReservationsToCsvAsync(string path, string filename, int bookId)

        [TestMethod]
        public async Task Write_BookReservations_toCSV_ListIsEmpty()
        {
            _mockUnitOfWork.Setup(x => x.ReservationRepository.GetBookReservationHistoryAsync(1)).Returns(Task.FromResult(new List<Reservation>() { }));
            _dataExportService = new DataExportService(_mockUnitOfWork.Object);

            await Assert.ThrowsExceptionAsync<OLNotFound>(() => _dataExportService.GetBookReservationsAsync(1));
            _mockUnitOfWork.Verify(x => x.ReservationRepository.GetBookReservationHistoryAsync(1), Times.Once);
        }

        [TestMethod]
        public async Task Write_BookReservations_toCSV_Ok()
        {
            Reservation reservation = new Reservation() { Id = 1, Book = new Book() { Id = 1, Name = "test" }, User = new User() { Id = 1, Name = "test" }, ReservationDate = DateTime.Now };
            _mockUnitOfWork.Setup(x => x.ReservationRepository.GetBookReservationHistoryAsync(1)).Returns(Task.FromResult(new List<Reservation>() { reservation }));
            _dataExportService = new DataExportService(_mockUnitOfWork.Object);

            await _dataExportService.GetBookReservationsAsync(1);
            _mockUnitOfWork.Verify(x => x.ReservationRepository.GetBookReservationHistoryAsync(1), Times.Once);
        }

        // Task WriteUserReservationsToCsvAsync(string path, string filename, int userId)

        [TestMethod]
        public async Task Write_UserReservations_toCSV_ListIsEmpty()
        {
            _mockUnitOfWork.Setup(x => x.ReservationRepository.GetUserReservationHistoryAsync(1)).Returns(Task.FromResult(new List<Reservation>() { }));
            _dataExportService = new DataExportService(_mockUnitOfWork.Object);

            await Assert.ThrowsExceptionAsync<OLNotFound>(() => _dataExportService.GetUserReservationsAsync(1));
            _mockUnitOfWork.Verify(x => x.ReservationRepository.GetUserReservationHistoryAsync(1), Times.Once);
        }

        [TestMethod]
        public async Task Write_UserReservations_toCSV_Ok()
        {
            Reservation reservation = new Reservation() { Id = 1, Book = new Book() { Id = 1, Name = "test" }, User = new User() { Id = 1, Name = "test" }, ReservationDate = DateTime.Now };
            _mockUnitOfWork.Setup(x => x.ReservationRepository.GetUserReservationHistoryAsync(1)).Returns(Task.FromResult(new List<Reservation>() { reservation }));
            _dataExportService = new DataExportService(_mockUnitOfWork.Object);

            await _dataExportService.GetUserReservationsAsync(1);
            _mockUnitOfWork.Verify(x => x.ReservationRepository.GetUserReservationHistoryAsync(1), Times.Once);
        }
    }
}
