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
        [DataRow(-1, " ", null, 0)]
        [DataRow(0, "", 1, null)]
        [DataRow(0, "", null, null)]
        [DataRow(0, "", -1, 2)]
        public void Filter_Books_NameAndAuthorIsEmpty_ListIsEmpty(int? authorId, string name, int? inReserve, int? InArchive)
        {
            mockUnitOfWork.Setup(x => x.Book.GetAllBooks()).Returns(new List<Book>() { });
            bookService = new BookService(mockUnitOfWork.Object);

            Assert.ThrowsException<Exception>(()=>bookService.FilterBooks(authorId, name, inReserve, InArchive), "Expected exception");
            mockUnitOfWork.Verify(x => x.Book.GetAllBooks(), Times.Once);
        }

        [TestMethod]
        [DataRow(1, " ", null, 0)]
        [DataRow(2, "", 1, null)]
        [DataRow(2, null, 1, 1)]
        public void Filter_Books_NameFieldIsEmpty_ListIsEmpty(int? authorId, string name, int? inReserve, int? InArchive)
        {
            mockUnitOfWork.Setup(x => x.Book.FilterBooks((int)authorId)).Returns(new List<Book>() { });
            bookService = new BookService(mockUnitOfWork.Object);

            Assert.ThrowsException<Exception>(() => bookService.FilterBooks(authorId, name, inReserve, InArchive), "Expected exception");
            mockUnitOfWork.Verify(x => x.Book.FilterBooks((int)authorId), Times.Once);
        }

        [TestMethod]
        [DataRow(-1, " s ", null, 0)]
        [DataRow(0, "a", 1, null)]
        [DataRow(null, "A", 1, 1)]
        public void Filter_Books_AuthorFieldIsEmpty_ListIsEmpty(int? authorId, string name, int? inReserve, int? InArchive)
        {
            mockUnitOfWork.Setup(x => x.Book.FilterBooks(name)).Returns(new List<Book>() { });
            bookService = new BookService(mockUnitOfWork.Object);

            Assert.ThrowsException<Exception>(() => bookService.FilterBooks(authorId, name, inReserve, InArchive), "Expected exception");
            mockUnitOfWork.Verify(x => x.Book.FilterBooks(name.Trim()), Times.Once);
        }

        [TestMethod]
        [DataRow(1, " s ", null, 0)]
        [DataRow(2, "a", 1, null)]
        [DataRow(3, "A", 1, 1)]
        public void Filter_Books_ListIsEmpty(int? authorId, string name, int? inReserve, int? InArchive)
        {
            mockUnitOfWork.Setup(x => x.Book.FilterBooks((int)authorId, name)).Returns(new List<Book>() { });
            bookService = new BookService(mockUnitOfWork.Object);

            Assert.ThrowsException<Exception>(() => bookService.FilterBooks(authorId, name, inReserve, InArchive), "Expected exception");
            mockUnitOfWork.Verify(x => x.Book.FilterBooks((int)authorId, name.Trim()), Times.Once);
        }

        [TestMethod]
        [DataRow(-1, " ", null, 0)]
        [DataRow(0, "", 1, null)]
        [DataRow(0, "", 0, 2)]
        [DataRow(0, "", -1, 1)]
        public void Filter_Books_NameAndAuthorIsEmpty_ReserveOrInArchive_Ok(int? authorId, string name, int? inReserve, int? InArchive)
        {
            List<Book> books = new List<Book>() { new Book() { Reserve= true, InArchive = true }, new Book() { } };
            mockUnitOfWork.Setup(x => x.Book.GetAllBooks()).Returns(books);
            bookService = new BookService(mockUnitOfWork.Object);

            List<Book> result = bookService.FilterBooks(authorId, name, inReserve, InArchive);
            int expectedCount = 1;
            Assert.AreEqual(expectedCount, result.Count);
            mockUnitOfWork.Verify(x => x.Book.GetAllBooks(), Times.Once);
        }
        [TestMethod]
        [DataRow(-1, " s", 1, 0)]
        [DataRow(0, "s ", 1, 1)]
        [DataRow(0, " s  ", 0, 0)]
        [DataRow(0, "s", 0, 1)]
        public void Filter_Books_AuthorIsEmpty_ReserveAndInArchive_Ok(int? authorId, string name, int? inReserve, int? InArchive)
        {
            List<Book> books = new List<Book>() { 
                new Book() { Name=name, Reserve = true, InArchive = true }, new Book() {Name=name },
                new Book() { Name=name, Reserve = true}, new Book() { Name=name, InArchive = true },
            };

            mockUnitOfWork.Setup(x => x.Book.FilterBooks(name.Trim())).Returns(books);
            bookService = new BookService(mockUnitOfWork.Object);

            List<Book> result = bookService.FilterBooks(authorId, name, inReserve, InArchive);
            int expectedCount = 1;
            Assert.AreEqual(expectedCount, result.Count);
            mockUnitOfWork.Verify(x => x.Book.FilterBooks(name.Trim()), Times.Once);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(null)]
        public void Get_BookById_IdIsIncorrect(int? id)
        {
            bookService = new BookService(mockUnitOfWork.Object);
            Assert.ThrowsException<Exception>(() => bookService.GetBookById(id), "Expected exception");
            mockUnitOfWork.Verify(x => x.Book.GetBookById(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        public void Get_BookById_BookNotFound(int? id)
        {
            mockUnitOfWork.Setup(x => x.Book.GetBookById((int)id)).Returns(value: null);
            bookService = new BookService(mockUnitOfWork.Object);
            Assert.ThrowsException<Exception>(() => bookService.GetBookById(id), "Expected exception");
            mockUnitOfWork.Verify(x => x.Book.GetBookById((int)id), Times.Once);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        public void Get_BookById_Ok(int? id)
        {
            Book book = new Book();
            mockUnitOfWork.Setup(x => x.Book.GetBookById((int)id)).Returns(book);
            bookService = new BookService(mockUnitOfWork.Object);
            Book actualBook = bookService.GetBookById(id);
            
            Assert.AreEqual(book, actualBook);
            mockUnitOfWork.Verify(x => x.Book.GetBookById((int)id), Times.Once);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(null)]
        [DataRow(0)]
        public void Change_BookReservation_IdIsIncorrect(int? id)
        {
            bookService = new BookService(mockUnitOfWork.Object);

            Assert.ThrowsException<Exception>(() => bookService.ChangeBookReservation(id, true), "Expected exception");
            mockUnitOfWork.Verify(x => x.Book.ChangeBookReservation(It.IsAny<int>(), It.IsAny<bool>()), Times.Never);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        public void Change_BookReservation_BookNotFound(int? id)
        {
            mockUnitOfWork.Setup(x => x.Book.IsBookIdExists(It.IsAny<int>())).Returns(false);
            bookService = new BookService(mockUnitOfWork.Object);

            Assert.ThrowsException<Exception>(() => bookService.ChangeBookReservation(id, true), "Expected exception");
            mockUnitOfWork.Verify(x => x.Book.ChangeBookReservation(It.IsAny<int>(), It.IsAny<bool>()), Times.Never);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        public void Change_BookReservation_Ok(int? id)
        {
            mockUnitOfWork.Setup(x => x.Book.ChangeBookReservation(It.IsAny<int>(), It.IsAny<bool>()));
            mockUnitOfWork.Setup(x => x.Book.IsBookIdExists(It.IsAny<int>())).Returns(true);
            bookService = new BookService(mockUnitOfWork.Object);

            bookService.ChangeBookReservation(id, true);
            mockUnitOfWork.Verify(x => x.Book.ChangeBookReservation(It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(null)]
        [DataRow(0)]
        public void Change_BookArchievation_IdIsIncorrect(int? id)
        {
            bookService = new BookService(mockUnitOfWork.Object);

            Assert.ThrowsException<Exception>(() => bookService.ChangeBookArchievation(id, true), "Expected exception");
            mockUnitOfWork.Verify(x => x.Book.ChangeBookArchievation(It.IsAny<int>(), It.IsAny<bool>()), Times.Never);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        public void Change_BookArchievation_BookNotFound(int? id)
        {
            mockUnitOfWork.Setup(x => x.Book.IsBookIdExists(It.IsAny<int>())).Returns(false);
            bookService = new BookService(mockUnitOfWork.Object);

            Assert.ThrowsException<Exception>(() => bookService.ChangeBookArchievation(id, true), "Expected exception");
            mockUnitOfWork.Verify(x => x.Book.ChangeBookArchievation(It.IsAny<int>(), It.IsAny<bool>()), Times.Never);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        public void Change_BookArchievation_Ok(int? id)
        {
            mockUnitOfWork.Setup(x => x.Book.ChangeBookArchievation(It.IsAny<int>(), It.IsAny<bool>()));
            mockUnitOfWork.Setup(x => x.Book.IsBookIdExists(It.IsAny<int>())).Returns(true);
            bookService = new BookService(mockUnitOfWork.Object);

            bookService.ChangeBookArchievation(id, true);
            mockUnitOfWork.Verify(x => x.Book.ChangeBookArchievation(It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
        }
    }
}
