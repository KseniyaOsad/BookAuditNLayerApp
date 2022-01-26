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
using System.ComponentModel;
using System.Linq.Expressions;

namespace OnlineLibraryApiTest.Services
{
    [TestClass]
    public class BookServiceTest
    {
        private BookService _bookService;

        private Mock<IUnitOfWork> _mockUnitOfWork;
        
        private Mock<IValidator<Book>> _mockBookValidator;

        private BookValidator _bookValidator;

        private Mock<IBookRepository> _mockBookRepository;

        private Mock<IAuthorRepository> _mockAuthorRepository;

        [TestInitialize]
        public void InitializeTest()
        {
            _mockBookValidator = new Mock<IValidator<Book>>();
            _bookValidator = new BookValidator();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockBookRepository = new Mock<IBookRepository>();
            _mockAuthorRepository = new Mock<IAuthorRepository>();
            _mockUnitOfWork.Setup(x => x.BookRepository).Returns(_mockBookRepository.Object);
            _mockUnitOfWork.Setup(x => x.AuthorRepository).Returns(_mockAuthorRepository.Object);
        }

        [TestMethod]
        public void UpdatePatch_Book_NotFound()
        {
            _mockUnitOfWork.Setup(x => x.BookRepository.GetBookById(It.IsAny<int>())).Returns(value: null);
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookValidator.Object);
            
            Assert.ThrowsException<OLNotFound>(() => _bookService.UpdatePatch(It.IsAny<int>(), It.IsAny<JsonPatchDocument<Book>>()), "Expected exception");
            _mockUnitOfWork.Verify(x => x.Save(), Times.Never);
        }

        [TestMethod]
        public void UpdatePatch_Book_NotValid()
        {
            JsonPatchDocument<Book> testBook = new JsonPatchDocument<Book>();
            Book book = new Book();
            _mockUnitOfWork.Setup(x => x.BookRepository.GetBookById(It.IsAny<int>())).Returns(book);
            var results = _bookValidator.Validate(book);
            _mockBookValidator.Setup(x=>x.Validate(It.IsAny<Book>())).Returns(results);
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookValidator.Object);

            Assert.ThrowsException<OLBadRequest>(() => _bookService.UpdatePatch(It.IsAny<int>(), testBook), "Expected exception");
            _mockUnitOfWork.Verify(x => x.Save(), Times.Never);
        }

        [TestMethod]
        [DataRow(null, "   ", -2)]
        [DataRow("", null, 0)]
        [DataRow("  ", null, 90)]
        public void Validate_Book_FieldIsIncorrect(string name, string descr, Genre genre)
        {
            Book book = new Book() { Name = name, Description = descr, Genre = genre, Authors = new List<Author> { }, Tags = new List<Tag> { } };
            var result = _bookValidator.TestValidate(book);
            result.ShouldHaveValidationErrorFor(x => x.Name);
            result.ShouldHaveValidationErrorFor(x => x.Description);
            result.ShouldHaveValidationErrorFor(x => x.Genre);
            result.ShouldHaveValidationErrorFor(x => x.Authors);
            result.ShouldHaveValidationErrorFor(x => x.Tags);
        }

        [TestMethod]
        public void Get_AllBooks_WithPagination_ListIsEmpty_Ok()
        {
            _mockUnitOfWork.Setup(x => x.BookRepository.GetAllBooksCount()).Returns(0);
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookValidator.Object);
            PaginatedList<Book> result = _bookService.GetAllBooks(It.IsAny<PaginationOptions>());
            Assert.AreEqual(0, result.TotalCount);
            _mockUnitOfWork.Verify(x => x.BookRepository.GetAllBooks(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        [DataRow(1, 2)]
        [DataRow(10, -2)]
        [DataRow(10, 20)]
        public void Get_AllBooks_WithPagination_Ok(int pNumber, int pageSize)
        {
            PaginationOptions pO = new PaginationOptions(pNumber, pageSize);
            _mockUnitOfWork.Setup(x => x.BookRepository.GetAllBooksCount()).Returns(1);
            _mockUnitOfWork.Setup(x => x.BookRepository.GetAllBooks(It.IsAny<int>(), pO.PageSize)).Returns(new List<Book>() { new Book() });
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookValidator.Object);
            PaginatedList<Book> result = _bookService.GetAllBooks(pO);
            Assert.AreEqual(1, result.TotalCount);
            _mockUnitOfWork.Verify(x => x.BookRepository.GetAllBooks(It.IsAny<int>(), pO.PageSize), Times.Once);
        }

        [TestMethod]
        [DataRow(-1, " ", null, 0, 1, 2)]
        [DataRow(0, "s", 1, null, -1, 12)]
        [DataRow(0, "", null, null, 10, 2)]
        [DataRow(1, "", -1, 2, 1, -2)]
        public void Filter_Books_WithPagination_ListIsEmpty(int? authorId, string name, int? inReserve, int? InArchive, int pNumber, int pageSize)
        {
            FilterBook filterBook = new FilterBook() { AuthorId = authorId, Name= name, Reservation= inReserve, Archievation= InArchive, TagId = authorId, Pagination = new PaginationOptions(pNumber, pageSize) };
            _mockUnitOfWork.Setup(x => x.BookRepository.GetAllBooksCount(It.IsAny<Expression<Func<Book, bool>>>())).Returns(0);
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookValidator.Object);

            Assert.ThrowsException<OLNotFound>(() => _bookService.FilterBooks(filterBook), "Expected exception");
            _mockUnitOfWork.Verify(x => x.BookRepository.FilterBooks(It.IsAny<Expression<Func<Book, bool>>>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<ListSortDirection>()), Times.Never);
        }

        [TestMethod]
        [DataRow(-1, " ", null, 0, 1, 2)]
        [DataRow(0, "s", 1, null, -1, 12)]
        [DataRow(0, "", null, null, 10, 2)]
        [DataRow(1, "", -1, 2, 1, -2)]
        public void Filter_Books_WithPagination_OK(int? authorId, string name, int? inReserve, int? InArchive, int pNumber, int pageSize)
        {
            FilterBook filterBook = new FilterBook() { AuthorId = authorId, Name= name, Reservation= inReserve, Archievation= InArchive, TagId = authorId, Pagination = new PaginationOptions(pNumber, pageSize) };
            _mockUnitOfWork.Setup(x => x.BookRepository.GetAllBooksCount(It.IsAny<Expression<Func<Book, bool>>>())).Returns(1);
            _mockUnitOfWork.Setup(x => x.BookRepository.FilterBooks(It.IsAny<Expression<Func<Book, bool>>>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<ListSortDirection>())).Returns(new List<Book>() { new Book() });
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookValidator.Object);

            PaginatedList<Book> result = _bookService.FilterBooks(filterBook);
            Assert.AreEqual(1, result.TotalCount);
            _mockUnitOfWork.Verify(x => x.BookRepository.FilterBooks(It.IsAny<Expression<Func<Book, bool>>>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<ListSortDirection>()), Times.Once);
        }
        
        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(null)]
        public void Get_BookById_IdIsIncorrect(int? id)
        {
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookValidator.Object);
            Assert.ThrowsException<OLBadRequest>(() => _bookService.GetBookById(id), "Expected exception");
            _mockUnitOfWork.Verify(x => x.BookRepository.GetBookById(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        public void Get_BookById_BookNotFound(int? id)
        {
            _mockUnitOfWork.Setup(x => x.BookRepository.GetBookById((int)id)).Returns(value: null);
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookValidator.Object);
            Assert.ThrowsException<OLNotFound>(() => _bookService.GetBookById(id), "Expected exception");
            _mockUnitOfWork.Verify(x => x.BookRepository.GetBookById((int)id), Times.Once);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        public void Get_BookById_Ok(int? id)
        {
            Book book = new Book();
            _mockUnitOfWork.Setup(x => x.BookRepository.GetBookById((int)id)).Returns(book);
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookValidator.Object);
            Book actualBook = _bookService.GetBookById(id);
            
            Assert.AreEqual(book, actualBook);
            _mockUnitOfWork.Verify(x => x.BookRepository.GetBookById((int)id), Times.Once);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(null)]
        [DataRow(0)]
        public void Change_BookReservation_IdIsIncorrect(int? id)
        {
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookValidator.Object);

            Assert.ThrowsException<OLBadRequest>(() => _bookService.ChangeBookReservation(id, true), "Expected exception");
            _mockUnitOfWork.Verify(x => x.BookRepository.ChangeBookReservation(It.IsAny<int>(), It.IsAny<bool>()), Times.Never);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        public void Change_BookReservation_BookNotFound(int? id)
        {
            _mockUnitOfWork.Setup(x => x.BookRepository.IsBookIdExists(It.IsAny<int>())).Returns(false);
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookValidator.Object);

            Assert.ThrowsException<OLNotFound>(() => _bookService.ChangeBookReservation(id, true), "Expected exception");
            _mockUnitOfWork.Verify(x => x.BookRepository.ChangeBookReservation(It.IsAny<int>(), It.IsAny<bool>()), Times.Never);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        public void Change_BookReservation_Ok(int? id)
        {
            _mockUnitOfWork.Setup(x => x.BookRepository.ChangeBookReservation(It.IsAny<int>(), It.IsAny<bool>()));
            _mockUnitOfWork.Setup(x => x.BookRepository.IsBookIdExists(It.IsAny<int>())).Returns(true);
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookValidator.Object);

            _bookService.ChangeBookReservation(id, true);
            _mockUnitOfWork.Verify(x => x.BookRepository.ChangeBookReservation(It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(null)]
        [DataRow(0)]
        public void Change_BookArchievation_IdIsIncorrect(int? id)
        {
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookValidator.Object);

            Assert.ThrowsException<OLBadRequest>(() => _bookService.ChangeBookArchievation(id, true), "Expected exception");
            _mockUnitOfWork.Verify(x => x.BookRepository.ChangeBookArchievation(It.IsAny<int>(), It.IsAny<bool>()), Times.Never);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        public void Change_BookArchievation_BookNotFound(int? id)
        {
            _mockUnitOfWork.Setup(x => x.BookRepository.IsBookIdExists(It.IsAny<int>())).Returns(false);
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookValidator.Object);

            Assert.ThrowsException<OLNotFound>(() => _bookService.ChangeBookArchievation(id, true), "Expected exception");
            _mockUnitOfWork.Verify(x => x.BookRepository.ChangeBookArchievation(It.IsAny<int>(), It.IsAny<bool>()), Times.Never);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        public void Change_BookArchievation_Ok(int? id)
        {
            _mockUnitOfWork.Setup(x => x.BookRepository.ChangeBookArchievation(It.IsAny<int>(), It.IsAny<bool>()));
            _mockUnitOfWork.Setup(x => x.BookRepository.IsBookIdExists(It.IsAny<int>())).Returns(true);
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookValidator.Object);

            _bookService.ChangeBookArchievation(id, true);
            _mockUnitOfWork.Verify(x => x.BookRepository.ChangeBookArchievation(It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        [DataRow("  ", "  ")]
        [DataRow("s", null)]
        [DataRow("", "s")]
        public void Create_Book_FieldsIsIncorrect(string name, string description)
        {
            _mockBookRepository.Setup(x => x.InsertBook(It.IsAny<Book>()));
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookValidator.Object);
            Book book = new Book() { Name = name, Description = description};
            Assert.ThrowsException<OLInternalServerError>(() => _bookService.CreateBook(book), "Expected exception");
            _mockUnitOfWork.Verify(x => x.BookRepository.InsertBook(It.IsAny<Book>()), Times.Once);
        }

        [TestMethod]
        [DataRow(" s ", " s ")]
        public void Create_Book_Ok(string name, string description)
        {
            _mockUnitOfWork.Setup(x => x.BookRepository.InsertBook(It.IsAny<Book>()));
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookValidator.Object);
            Book book = new Book() { Id = 1, Name = name, Description = description, Authors = new List<Author>() { new Author() }, Tags = new List<Tag>() { new Tag() }, Genre = Genre.Adventures };
            _bookService.CreateBook(book);
            _mockUnitOfWork.Verify(x => x.BookRepository.InsertBook(It.IsAny<Book>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);

        }
    }
}
