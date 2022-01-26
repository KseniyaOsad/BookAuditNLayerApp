using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OnlineLibrary.BLL.Services;
using OnlineLibrary.Common.Entities;
using OnlineLibrary.Common.Exceptions;
using OnlineLibrary.DAL.Interfaces;
using System;
using System.Collections.Generic;

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
        public void Write_toCSV_ListIsEmpty()
        {
            _mockUnitOfWork.Setup(x => x.BookRepository.GetAllBooks(It.IsAny<int>(), It.IsAny<int>())).Returns(new List<Book>() {  });
            _dataExportService = new DataExportService(_mockUnitOfWork.Object);

            Assert.ThrowsException<OLNotFound>(() => _dataExportService.WriteCsv("", ""), "Expected Exception");
            _mockUnitOfWork.Verify(x => x.BookRepository.GetAllBooks(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        [DataRow("", "")]
        [DataRow("Hi", "")]
        [DataRow("", null)]
        [DataRow("Hi", "/Hi")]
        public void Write_toCSV_PathIsINcorrect(string path, string filename)
        {
            _mockUnitOfWork.Setup(x => x.BookRepository.GetAllBooks(It.IsAny<int>(), It.IsAny<int>())).Returns(new List<Book>() { new Book() });
            _dataExportService = new DataExportService(_mockUnitOfWork.Object);

            Assert.ThrowsException<OLInternalServerError>(() => _dataExportService.WriteCsv(path, filename), "Expected Exception");
            _mockUnitOfWork.Verify(x => x.BookRepository.GetAllBooks(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        public void Write_toCSV_Ok()
        {
            _mockUnitOfWork.Setup(x => x.BookRepository.GetAllBooks(It.IsAny<int>(), It.IsAny<int>())).Returns(new List<Book>() { new Book() { Name = "Hello world", Authors = new List<Author> { new Author() { Name="me" } } } });
            _dataExportService = new DataExportService(_mockUnitOfWork.Object);
            _dataExportService.WriteCsv(_path, _fileName);
            _mockUnitOfWork.Verify(x => x.BookRepository.GetAllBooks(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }
    }
}
