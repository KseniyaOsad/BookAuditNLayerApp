using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OnlineLibrary.BLL.Services;
using OnlineLibrary.Common.Entities;
using OnlineLibrary.Common.Exceptions;
using OnlineLibrary.DAL.Interfaces;
using System;
using System.Collections.Generic;

namespace OnlineLibraryApiTest
{
    [TestClass]
    public class DataExportServiceTest
    {
        private DataExportService dataExportService;

        private Mock<IUnitOfWork> mockUnitOfWork;

        private Mock<IBookRepository> mockBookRepository;

        private Mock<IAuthorRepository> mockAuthorRepository;

        private const string _fileName = "book.csv";

        private readonly string _path = @"C:\Users\theks\Desktop\C\OnlineLibrary\OnlineLibraryApiTest\Data\";

        [TestInitialize]
        public void InitializeTest()
        {
            mockUnitOfWork = new Mock<IUnitOfWork>();
            mockBookRepository = new Mock<IBookRepository>();
            mockAuthorRepository = new Mock<IAuthorRepository>();
            mockUnitOfWork.Setup(x => x.BookRepository).Returns(mockBookRepository.Object);
            mockUnitOfWork.Setup(x => x.AuthorRepository).Returns(mockAuthorRepository.Object);
        }

        [TestMethod]
        public void Write_toCSV_ListIsEmpty()
        {
            mockUnitOfWork.Setup(x => x.BookRepository.GetAllBooks()).Returns(new List<Book>() {  });
            dataExportService = new DataExportService(mockUnitOfWork.Object);

            Assert.ThrowsException<OLNotFound>(() => dataExportService.WriteCsv("", ""), "Expected Exception");
            mockUnitOfWork.Verify(x => x.BookRepository.GetAllBooks(), Times.Once);
        }

        [TestMethod]
        [DataRow("", "")]
        [DataRow("Hi", "")]
        [DataRow("", null)]
        [DataRow("Hi", "/Hi")]
        public void Write_toCSV_PathIsINcorrect(string path, string filename)
        {
            mockUnitOfWork.Setup(x => x.BookRepository.GetAllBooks()).Returns(new List<Book>() { new Book() });
            dataExportService = new DataExportService(mockUnitOfWork.Object);

            Assert.ThrowsException<OLInternalServerError>(() => dataExportService.WriteCsv(path, filename), "Expected Exception");
            mockUnitOfWork.Verify(x => x.BookRepository.GetAllBooks(), Times.Once);
        }

        [TestMethod]
        public void Write_toCSV_Ok()
        {
            mockUnitOfWork.Setup(x => x.BookRepository.GetAllBooks()).Returns(new List<Book>() { new Book() { Name = "Hello world", Authors = new List<Author> { new Author() { Name="me" } } } });
            dataExportService = new DataExportService(mockUnitOfWork.Object);
            dataExportService.WriteCsv(_path, _fileName);
            mockUnitOfWork.Verify(x => x.BookRepository.GetAllBooks(), Times.Once);
        }
    }
}
