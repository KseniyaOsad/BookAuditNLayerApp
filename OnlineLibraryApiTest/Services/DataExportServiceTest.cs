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

        private Mock<IUnitOfWork> _mockUnitOfWork;

        private Mock<IBookRepository> _mockBookRepository;

        private Mock<IAuthorRepository> _mockAuthorRepository;

        private const string _fileName = "book.csv";

        private readonly string _path = @"C:\Users\theks\Desktop\C\OnlineLibrary\OnlineLibraryApiTest\Data\";

        [TestInitialize]
        public void InitializeTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockBookRepository = new Mock<IBookRepository>();
            _mockAuthorRepository = new Mock<IAuthorRepository>();
            _mockUnitOfWork.Setup(x => x.BookRepository).Returns(_mockBookRepository.Object);
            _mockUnitOfWork.Setup(x => x.AuthorRepository).Returns(_mockAuthorRepository.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(OLNotFound))]
        public async Task Write_toCSV_ListIsEmpty()
        {
            _mockUnitOfWork.Setup(x => x.BookRepository.GetAllBooksAsync(It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(new List<Book>() {  }));
            _dataExportService = new DataExportService(_mockUnitOfWork.Object);

            await _dataExportService.WriteCsvAsync("", "");
            _mockUnitOfWork.Verify(x => x.BookRepository.GetAllBooksAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        [DataRow("", "")]
        [DataRow("Hi", "")]
        [DataRow("", null)]
        [DataRow("Hi", "/Hi")]
        [ExpectedException(typeof(OLInternalServerError))]
        public async Task Write_toCSV_PathIsINcorrect(string path, string filename)
        {
            _mockUnitOfWork.Setup(x => x.BookRepository.GetAllBooksCountAsync()).Returns(Task.FromResult(1));
            _mockUnitOfWork.Setup(x => x.BookRepository.GetAllBooksAsync(It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(new List<Book>() { new Book() }));
            _dataExportService = new DataExportService(_mockUnitOfWork.Object);

            await _dataExportService.WriteCsvAsync(path, filename);
            _mockUnitOfWork.Verify(x => x.BookRepository.GetAllBooksAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        public async Task Write_toCSV_Ok()
        {
            _mockUnitOfWork.Setup(x => x.BookRepository.GetAllBooksCountAsync()).Returns(Task.FromResult(1));
            _mockUnitOfWork.Setup(x => x.BookRepository.GetAllBooksAsync(It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(new List<Book>() { new Book() { Name = "Hello world", Authors = new List<Author> { new Author() { Name="me" } } } }));
            _dataExportService = new DataExportService(_mockUnitOfWork.Object);
            await _dataExportService.WriteCsvAsync(_path, _fileName);
            _mockUnitOfWork.Verify(x => x.BookRepository.GetAllBooksAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }
    }
}
