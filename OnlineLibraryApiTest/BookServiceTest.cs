using FluentValidation;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OnlineLibrary.BLL.Services;
using OnlineLibrary.Common.Entities;
using OnlineLibrary.Common.Exceptions;
using OnlineLibrary.Common.Pagination;
using OnlineLibrary.Common.Validators;
using OnlineLibrary.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OnlineLibraryApiTest
{
    [TestClass]
    public class BookServiceTest
    {
        private BookService bookService;

        private Mock<IUnitOfWork> mockUnitOfWork;
        
        private Mock<IValidator<Book>> mockBookValidator;

        private BookValidator bookValidator;

        private Mock<IBookRepository> mockBookRepository;

        private Mock<IAuthorRepository> mockAuthorRepository;

        [TestInitialize]
        public void InitializeTest()
        {
            mockBookValidator = new Mock<IValidator<Book>>();
            bookValidator = new BookValidator();
            mockUnitOfWork = new Mock<IUnitOfWork>();
            mockBookRepository = new Mock<IBookRepository>();
            mockAuthorRepository = new Mock<IAuthorRepository>();
            mockUnitOfWork.Setup(x => x.BookRepository).Returns(mockBookRepository.Object);
            mockUnitOfWork.Setup(x => x.AuthorRepository).Returns(mockAuthorRepository.Object);
        }

        [TestMethod]
        public void UpdatePatch_Book_NotFound()
        {
            mockUnitOfWork.Setup(x => x.BookRepository.GetBookById(It.IsAny<int>())).Returns(value: null);
            bookService = new BookService(mockUnitOfWork.Object, mockBookValidator.Object);
            
            Assert.ThrowsException<OLNotFound>(() => bookService.UpdatePatch(It.IsAny<int>(), It.IsAny<JsonPatchDocument<Book>>()), "Expected exception");
            mockUnitOfWork.Verify(x => x.Save(), Times.Never);
        }

        [TestMethod]
        public void UpdatePatch_Book_NotValid()
        {
            JsonPatchDocument<Book> testBook = new JsonPatchDocument<Book>();
            Book book = new Book();
            mockUnitOfWork.Setup(x => x.BookRepository.GetBookById(It.IsAny<int>())).Returns(book);
            var results = bookValidator.Validate(book);
            mockBookValidator.Setup(x=>x.Validate(It.IsAny<Book>())).Returns(results);
            bookService = new BookService(mockUnitOfWork.Object, mockBookValidator.Object);

            Assert.ThrowsException<OLBadRequest>(() => bookService.UpdatePatch(It.IsAny<int>(), testBook), "Expected exception");
            mockUnitOfWork.Verify(x => x.Save(), Times.Never);
        }

        [TestMethod]
        [DataRow(null, "   ", -2)]
        [DataRow("", null, 0)]
        [DataRow("  ", null, 90)]
        public void Validate_Book_FieldIsIncorrect(string name, string descr, Genre genre)
        {
            Book book = new Book() { Name = name, Description = descr, Genre = genre, Authors = new List<Author> { } };
            var result = bookValidator.TestValidate(book);
            result.ShouldHaveValidationErrorFor(x => x.Name);
            result.ShouldHaveValidationErrorFor(x => x.Description);
            result.ShouldHaveValidationErrorFor(x => x.Genre);
            result.ShouldHaveValidationErrorFor(x => x.Authors);
        }

        [TestMethod]
        public void Get_AllBooks_Ok()
        {
            List<Book> books = new List<Book>() { };
            mockUnitOfWork.Setup(x => x.BookRepository.GetAllBooks()).Returns(books);
            bookService = new BookService(mockUnitOfWork.Object, mockBookValidator.Object);

            List<Book> result = bookService.GetAllBooks();
            Assert.AreEqual(books, result);
            mockUnitOfWork.Verify(x => x.BookRepository.GetAllBooks(), Times.Once);
        }

        [TestMethod]
        public void Get_AllBooks_WithPagination_ListIsEmpty_Ok()
        {
            mockUnitOfWork.Setup(x => x.BookRepository.GetAllBooksCount()).Returns(0);
            bookService = new BookService(mockUnitOfWork.Object, mockBookValidator.Object);
            PaginatedList<Book> result = bookService.GetAllBooks(It.IsAny<PaginationOptions>());
            Assert.AreEqual(0, result.TotalCount);
            mockUnitOfWork.Verify(x => x.BookRepository.GetAllBooks(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        [DataRow(1, 2)]
        [DataRow(10, -2)]
        [DataRow(10, 20)]
        public void Get_AllBooks_WithPagination_Ok(int pNumber, int pageSize)
        {
            PaginationOptions pO = new PaginationOptions(pNumber, pageSize);
            mockUnitOfWork.Setup(x => x.BookRepository.GetAllBooksCount()).Returns(1);
            mockUnitOfWork.Setup(x => x.BookRepository.GetAllBooks(It.IsAny<int>(), pO.PageSize)).Returns(new List<Book>() { new Book() });
            bookService = new BookService(mockUnitOfWork.Object, mockBookValidator.Object);
            PaginatedList<Book> result = bookService.GetAllBooks(pO);
            Assert.AreEqual(1, result.TotalCount);
            mockUnitOfWork.Verify(x => x.BookRepository.GetAllBooks(It.IsAny<int>(), pO.PageSize), Times.Once);
        }

        [TestMethod]
        [DataRow(-1, " ", null, 0)]
        [DataRow(0, "s", 1, null)]
        [DataRow(0, "", null, null)]
        [DataRow(1, "", -1, 2)]
        public void Filter_Books_ListIsEmpty(int? authorId, string name, int? inReserve, int? InArchive)
        {
            mockUnitOfWork.Setup(x => x.BookRepository.FilterBooks(It.IsAny<Expression<Func<Book, bool>>>())).Returns(new List<Book>() { });
            bookService = new BookService(mockUnitOfWork.Object, mockBookValidator.Object);

            Assert.ThrowsException<OLNotFound>(()=>bookService.FilterBooks(authorId, name, inReserve, InArchive), "Expected exception");
            mockUnitOfWork.Verify(x => x.BookRepository.FilterBooks(It.IsAny<Expression<Func<Book, bool>>>()), Times.Once);
        }

        [TestMethod]
        [DataRow(-1, " ", null, 0, 1, 2)]
        [DataRow(0, "s", 1, null, -1, 12)]
        [DataRow(0, "", null, null, 10, 2)]
        [DataRow(1, "", -1, 2, 1, -2)]
        public void Filter_Books_WithPagination_ListIsEmpty(int? authorId, string name, int? inReserve, int? InArchive, int pNumber, int pageSize)
        {
            PaginationOptions pO = new PaginationOptions(pNumber, pageSize);
            mockUnitOfWork.Setup(x => x.BookRepository.GetAllBooksCount(It.IsAny<Expression<Func<Book, bool>>>())).Returns(0);
            bookService = new BookService(mockUnitOfWork.Object, mockBookValidator.Object);

            Assert.ThrowsException<OLNotFound>(() => bookService.FilterBooks(authorId, name, inReserve, InArchive, pO), "Expected exception");
            mockUnitOfWork.Verify(x => x.BookRepository.FilterBooks(It.IsAny<Expression<Func<Book, bool>>>(), It.IsAny<int>(), pO.PageSize), Times.Never);
        }

        [TestMethod]
        [DataRow(-1, " ", null, 0)]
        [DataRow(0, "s", 1, null)]
        [DataRow(0, "", null, null)]
        [DataRow(1, "", -1, 2)]
        public void Filter_Books_OK(int? authorId, string name, int? inReserve, int? InArchive)
        {
            List<Book> books = new List<Book>() { new Book() };
            mockUnitOfWork.Setup(x => x.BookRepository.FilterBooks(It.IsAny<Expression<Func<Book, bool>>>())).Returns(books);
            bookService = new BookService(mockUnitOfWork.Object, mockBookValidator.Object);

            List<Book> result = bookService.FilterBooks(authorId, name, inReserve, InArchive);
            Assert.AreEqual(books, result);
            mockUnitOfWork.Verify(x => x.BookRepository.FilterBooks(It.IsAny<Expression<Func<Book, bool>>>()), Times.Once);
        }

        [TestMethod]
        [DataRow(-1, " ", null, 0, 1, 2)]
        [DataRow(0, "s", 1, null, -1, 12)]
        [DataRow(0, "", null, null, 10, 2)]
        [DataRow(1, "", -1, 2, 1, -2)]
        public void Filter_Books_WithPagination_OK(int? authorId, string name, int? inReserve, int? InArchive, int pNumber, int pageSize)
        {
            PaginationOptions pO = new PaginationOptions(pNumber, pageSize);
            mockUnitOfWork.Setup(x => x.BookRepository.GetAllBooksCount(It.IsAny<Expression<Func<Book, bool>>>())).Returns(1);
            mockUnitOfWork.Setup(x => x.BookRepository.FilterBooks(It.IsAny<Expression<Func<Book, bool>>>(), It.IsAny<int>(), pO.PageSize)).Returns(new List<Book>() { new Book() });
            bookService = new BookService(mockUnitOfWork.Object, mockBookValidator.Object);

            PaginatedList<Book> result = bookService.FilterBooks(authorId, name, inReserve, InArchive, pO);
            Assert.AreEqual(1, result.TotalCount);
            mockUnitOfWork.Verify(x => x.BookRepository.FilterBooks(It.IsAny<Expression<Func<Book, bool>>>(), It.IsAny<int>(), pO.PageSize), Times.Once);
        }
        
        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(null)]
        public void Get_BookById_IdIsIncorrect(int? id)
        {
            bookService = new BookService(mockUnitOfWork.Object, mockBookValidator.Object);
            Assert.ThrowsException<OLBadRequest>(() => bookService.GetBookById(id), "Expected exception");
            mockUnitOfWork.Verify(x => x.BookRepository.GetBookById(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        public void Get_BookById_BookNotFound(int? id)
        {
            mockUnitOfWork.Setup(x => x.BookRepository.GetBookById((int)id)).Returns(value: null);
            bookService = new BookService(mockUnitOfWork.Object, mockBookValidator.Object);
            Assert.ThrowsException<OLNotFound>(() => bookService.GetBookById(id), "Expected exception");
            mockUnitOfWork.Verify(x => x.BookRepository.GetBookById((int)id), Times.Once);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        public void Get_BookById_Ok(int? id)
        {
            Book book = new Book();
            mockUnitOfWork.Setup(x => x.BookRepository.GetBookById((int)id)).Returns(book);
            bookService = new BookService(mockUnitOfWork.Object, mockBookValidator.Object);
            Book actualBook = bookService.GetBookById(id);
            
            Assert.AreEqual(book, actualBook);
            mockUnitOfWork.Verify(x => x.BookRepository.GetBookById((int)id), Times.Once);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(null)]
        [DataRow(0)]
        public void Change_BookReservation_IdIsIncorrect(int? id)
        {
            bookService = new BookService(mockUnitOfWork.Object, mockBookValidator.Object);

            Assert.ThrowsException<OLBadRequest>(() => bookService.ChangeBookReservation(id, true), "Expected exception");
            mockUnitOfWork.Verify(x => x.BookRepository.ChangeBookReservation(It.IsAny<int>(), It.IsAny<bool>()), Times.Never);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        public void Change_BookReservation_BookNotFound(int? id)
        {
            mockUnitOfWork.Setup(x => x.BookRepository.IsBookIdExists(It.IsAny<int>())).Returns(false);
            bookService = new BookService(mockUnitOfWork.Object, mockBookValidator.Object);

            Assert.ThrowsException<OLNotFound>(() => bookService.ChangeBookReservation(id, true), "Expected exception");
            mockUnitOfWork.Verify(x => x.BookRepository.ChangeBookReservation(It.IsAny<int>(), It.IsAny<bool>()), Times.Never);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        public void Change_BookReservation_Ok(int? id)
        {
            mockUnitOfWork.Setup(x => x.BookRepository.ChangeBookReservation(It.IsAny<int>(), It.IsAny<bool>()));
            mockUnitOfWork.Setup(x => x.BookRepository.IsBookIdExists(It.IsAny<int>())).Returns(true);
            bookService = new BookService(mockUnitOfWork.Object, mockBookValidator.Object);

            bookService.ChangeBookReservation(id, true);
            mockUnitOfWork.Verify(x => x.BookRepository.ChangeBookReservation(It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(null)]
        [DataRow(0)]
        public void Change_BookArchievation_IdIsIncorrect(int? id)
        {
            bookService = new BookService(mockUnitOfWork.Object, mockBookValidator.Object);

            Assert.ThrowsException<OLBadRequest>(() => bookService.ChangeBookArchievation(id, true), "Expected exception");
            mockUnitOfWork.Verify(x => x.BookRepository.ChangeBookArchievation(It.IsAny<int>(), It.IsAny<bool>()), Times.Never);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        public void Change_BookArchievation_BookNotFound(int? id)
        {
            mockUnitOfWork.Setup(x => x.BookRepository.IsBookIdExists(It.IsAny<int>())).Returns(false);
            bookService = new BookService(mockUnitOfWork.Object, mockBookValidator.Object);

            Assert.ThrowsException<OLNotFound>(() => bookService.ChangeBookArchievation(id, true), "Expected exception");
            mockUnitOfWork.Verify(x => x.BookRepository.ChangeBookArchievation(It.IsAny<int>(), It.IsAny<bool>()), Times.Never);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        public void Change_BookArchievation_Ok(int? id)
        {
            mockUnitOfWork.Setup(x => x.BookRepository.ChangeBookArchievation(It.IsAny<int>(), It.IsAny<bool>()));
            mockUnitOfWork.Setup(x => x.BookRepository.IsBookIdExists(It.IsAny<int>())).Returns(true);
            bookService = new BookService(mockUnitOfWork.Object, mockBookValidator.Object);

            bookService.ChangeBookArchievation(id, true);
            mockUnitOfWork.Verify(x => x.BookRepository.ChangeBookArchievation(It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        [DataRow("  ", "  ")]
        [DataRow("s", null)]
        [DataRow("", "s")]
        public void Create_Book_FieldsIsIncorrect(string name, string description)
        {
            mockBookRepository.Setup(x => x.InsertBook(It.IsAny<Book>()));
            bookService = new BookService(mockUnitOfWork.Object, mockBookValidator.Object);
            Book book = new Book() { Name = name, Description = description};
            Assert.ThrowsException<OLInternalServerError>(() => bookService.CreateBook(book), "Expected exception");
            mockUnitOfWork.Verify(x => x.BookRepository.InsertBook(It.IsAny<Book>()), Times.Once);
        }

        [TestMethod]
        [DataRow(" s ", " s ")]
        public void Create_Book_Ok(string name, string description)
        {
            mockUnitOfWork.Setup(x => x.BookRepository.InsertBook(It.IsAny<Book>()));
            bookService = new BookService(mockUnitOfWork.Object, mockBookValidator.Object);
            Book book = new Book() { Id = 1, Name = name, Description = description, Authors = new List<Author>() { new Author() }, Genre = Genre.Adventures };
            bookService.CreateBook(book);
            mockUnitOfWork.Verify(x => x.BookRepository.InsertBook(It.IsAny<Book>()), Times.Once);
            mockUnitOfWork.Verify(x => x.Save(), Times.Once);

        }
    }
}
