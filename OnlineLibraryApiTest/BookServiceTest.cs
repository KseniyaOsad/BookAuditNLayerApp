using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OnlineLibrary.BLL.Services;
using OnlineLibrary.Common.Entities;
using OnlineLibrary.DAL.Interfaces;
using OnlineLibraryApiTest.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineLibraryApiTest
{
    [TestClass]
    public class BookServiceTest
    {
        private BookService bookService;
        private Mock<IUnitOfWork> mockUnitOfWork;

        [TestInitialize]
        public void InitializeTest()
        {
            mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(x => x.Book).Returns(new TestBookRepository());
            mockUnitOfWork.Setup(x => x.Author).Returns(new TestAuthorRepository());
        }

        [TestMethod]
        public void Get_AllBooks_ListIsEmpty()
        {
            mockUnitOfWork.Setup(x => x.Book.GetAllBooks()).Returns(new List<Book>() { });
            bookService = new BookService(mockUnitOfWork.Object);

            Assert.ThrowsException<Exception>(() => bookService.GetAllBooks(), "Expected Exception");
            mockUnitOfWork.Verify(x => x.Book.GetAllBooks(), Times.Once);
        }

        [TestMethod]
        public void Get_AllBooks_Ok()
        {
            List<Book> books = new List<Book>() { new Book() };
            mockUnitOfWork.Setup(x => x.Book.GetAllBooks()).Returns(books);
            bookService = new BookService(mockUnitOfWork.Object);

            List<Book> result = bookService.GetAllBooks();
            Assert.AreEqual(books, result);
            mockUnitOfWork.Verify(x => x.Book.GetAllBooks(), Times.Once);
        }
        
        [TestMethod]
        [DataRow()]
        public void Filter_Books_NameAndAuthorIsEmpty()
        {
            mockUnitOfWork.Setup(x => x.Book.GetAllBooks()).Returns(new List<Book>() { });

        }

    }
}
