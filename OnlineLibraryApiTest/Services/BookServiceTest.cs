using FluentValidation;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OnlineLibrary.BLL.Services;
using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.Common.Exceptions;
using OnlineLibrary.Common.EntityProcessing.Pagination;
using OnlineLibrary.Common.Validators;
using OnlineLibrary.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using OnlineLibrary.Common.Extensions;
using System.Linq;
using OnlineLibrary.Common.DBEntities.Enums;
using OnlineLibrary.Common.EntityProcessing;
using OnlineLibrary.Common.EntityProcessing.Sorting;
using System.Threading.Tasks;

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
        [ExpectedException(typeof(OLNotFound))]
        public async Task UpdatePatch_Book_NotFound()
        {
            Book book = null;
            _mockUnitOfWork.Setup(x => x.BookRepository.GetBookByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(book));
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookValidator.Object);
            
            await _bookService.UpdatePatchAsync(It.IsAny<int>(), It.IsAny<JsonPatchDocument<Book>>());
            _mockUnitOfWork.Verify(x => x.SaveAsync(), Times.Never);
        }

        [TestMethod]
        [ExpectedException(typeof(OLBadRequest))]
        public async Task UpdatePatch_Book_NotValid()
        {
            JsonPatchDocument<Book> testBook = new JsonPatchDocument<Book>();
            Book book = new Book();
            _mockUnitOfWork.Setup(x => x.BookRepository.GetBookByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(book));
            var results = _bookValidator.Validate(book);
            _mockBookValidator.Setup(x=>x.Validate(It.IsAny<Book>())).Returns(results);
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookValidator.Object);

            await _bookService.UpdatePatchAsync(It.IsAny<int>(), testBook);
            _mockUnitOfWork.Verify(x => x.SaveAsync(), Times.Never);
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
        public async Task Get_AllBooks_WithPagination_ListIsEmpty_Ok()
        {
            _mockUnitOfWork.Setup(x => x.BookRepository.GetAllBooksCountAsync()).Returns(Task.FromResult(0));
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookValidator.Object);
            PaginatedList<Book> result = await _bookService.GetAllBooksAsync(It.IsAny<PaginationOptions>());
            Assert.AreEqual(0, result.TotalCount);
            _mockUnitOfWork.Verify(x => x.BookRepository.GetAllBooksAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        [DataRow(1, 2)]
        [DataRow(10, -2)]
        [DataRow(10, 20)]
        public async Task Get_AllBooks_WithPagination_Ok(int pNumber, int pageSize)
        {
            PaginationOptions pO = new PaginationOptions(pNumber, pageSize);
            _mockUnitOfWork.Setup(x => x.BookRepository.GetAllBooksCountAsync()).Returns(Task.FromResult(1));
            _mockUnitOfWork.Setup(x => x.BookRepository.GetAllBooksAsync(It.IsAny<int>(), pO.PageSize)).Returns(Task.FromResult(new List<Book>() { new Book() }));
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookValidator.Object);
            PaginatedList<Book> result = await _bookService.GetAllBooksAsync(pO);
            Assert.AreEqual(1, result.TotalCount);
            _mockUnitOfWork.Verify(x => x.BookRepository.GetAllBooksAsync(It.IsAny<int>(), pO.PageSize), Times.Once);
        }

        [TestMethod]
        [DataRow("nme", -1)]
        [DataRow("", null)]
        [DataRow("name", 2)]
        [DataRow(" ", 2)]
        [DataRow(null, -2)]
        public async Task Filter_Books_CheckOrdering_OrderPropertyIsIncorrect(string propertyToOrder, ListSortDirection sortDirection)
        {
            BookProcessing bookProcessing = new BookProcessing();
            bookProcessing.Sorting = new SortingOptions() { SortDirection = sortDirection, PropertyToOrder = propertyToOrder };
            _mockUnitOfWork.Setup(x => x.BookRepository.GetAllBooksCountAsync(It.IsAny<Expression<Func<Book, bool>>>())).Returns(Task.FromResult(1));
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookValidator.Object);

            await _bookService.FilterBooksAsync(bookProcessing);
            _mockUnitOfWork.Verify(x => x.BookRepository.FilterBooksAsync(It.IsAny<Expression<Func<Book, bool>>>(), It.IsAny<int>(), It.IsAny<int>(), "Id", ListSortDirection.Ascending), Times.Once);
        }

        [TestMethod]
        [DataRow("Name", 1)]
        [DataRow("Id", 0)]
        [DataRow("Description", 0)]
        [DataRow("Reserve", 0)]
        [DataRow("InArchive", 1)]
        [DataRow("Genre", 1)]
        public async Task Filter_Books_CheckOrdering_Ok(string propertyToOrder, ListSortDirection sortDirection)
        {
            BookProcessing bookProcessing = new BookProcessing();
            bookProcessing.Sorting = new SortingOptions() { SortDirection = sortDirection, PropertyToOrder = propertyToOrder };
            _mockUnitOfWork.Setup(x => x.BookRepository.GetAllBooksCountAsync(It.IsAny<Expression<Func<Book, bool>>>())).Returns(Task.FromResult(1));
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookValidator.Object);

            await _bookService.FilterBooksAsync(bookProcessing);
            _mockUnitOfWork.Verify(x => x.BookRepository.FilterBooksAsync(It.IsAny<Expression<Func<Book, bool>>>(), It.IsAny<int>(), It.IsAny<int>(), propertyToOrder, sortDirection), Times.Once);
        }

        [TestMethod]
        [DataRow("name")]
        [DataRow("hi")]
        [DataRow("Nme")]
        public void Order_Book_OrderPropertyNotFound(string propertyToOrder)
        {
            IQueryable<Book> books = Enumerable.Empty<Book>().AsQueryable();
            Assert.ThrowsException<ArgumentNullException>(()=> books.OrderBy(propertyToOrder, ListSortDirection.Ascending), "Expected exception");
        }

        [TestMethod]
        [DataRow("Name")]
        [DataRow("Id")]
        [DataRow("Description")]
        [DataRow("Reserve")]
        [DataRow("InArchive")]
        [DataRow("Genre")]
        public void Order_Book_Ok(string propertyToOrder)
        {
            IQueryable<Book> books = Enumerable.Empty<Book>().AsQueryable();
            // No exceptions while trying to sort.   
            books.OrderBy(propertyToOrder, ListSortDirection.Ascending);
        }

        [TestMethod]
        [DataRow( 1, 2)]
        [DataRow(-1, 12)]
        [DataRow( 10, 2)]
        [DataRow(1, -2)]
        [ExpectedException(typeof(OLNotFound))]
        public async Task Filter_Books_WithPagination_ListIsEmpty( int pNumber, int pageSize)
        {
            BookProcessing bookProcessing = new BookProcessing();
            bookProcessing.Pagination = new PaginationOptions(pNumber, pageSize);

            _mockUnitOfWork.Setup(x => x.BookRepository.GetAllBooksCountAsync(It.IsAny<Expression<Func<Book, bool>>>())).Returns(Task.FromResult(0));
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookValidator.Object);

            await _bookService.FilterBooksAsync(bookProcessing);
            _mockUnitOfWork.Verify(x => x.BookRepository.FilterBooksAsync(It.IsAny<Expression<Func<Book, bool>>>(), It.IsAny<int>(), It.IsAny<int>(), "Id", ListSortDirection.Ascending), Times.Never);
        }

        [TestMethod]
        public async Task Filter_Books_WithPagination_OK()
        {
            BookProcessing bookProcessing = new BookProcessing();
            _mockUnitOfWork.Setup(x => x.BookRepository.GetAllBooksCountAsync(It.IsAny<Expression<Func<Book, bool>>>())).Returns(Task.FromResult(1));
            _mockUnitOfWork.Setup(x => x.BookRepository.FilterBooksAsync(It.IsAny<Expression<Func<Book, bool>>>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<ListSortDirection>())).Returns(Task.FromResult(new List<Book>() { new Book() }));
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookValidator.Object);

            PaginatedList<Book> result = await _bookService.FilterBooksAsync(bookProcessing);
            Assert.AreEqual(1, result.TotalCount);
            _mockUnitOfWork.Verify(x => x.BookRepository.FilterBooksAsync(It.IsAny<Expression<Func<Book, bool>>>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<ListSortDirection>()), Times.Once);
        }
        
        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(null)]
        [ExpectedException(typeof(OLBadRequest))]
        public async Task Get_BookById_IdIsIncorrect(int? id)
        {
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookValidator.Object);
            await _bookService.GetBookByIdAsync(id);
            _mockUnitOfWork.Verify(x => x.BookRepository.GetBookByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [ExpectedException(typeof(OLNotFound))]
        public async Task Get_BookById_BookNotFound(int id)
        {
            Book book = null;
            _mockUnitOfWork.Setup(x => x.BookRepository.GetBookByIdAsync(id)).Returns(Task.FromResult(book));
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookValidator.Object);
            await _bookService.GetBookByIdAsync(id);
            _mockUnitOfWork.Verify(x => x.BookRepository.GetBookByIdAsync(id), Times.Once);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        public async Task Get_BookById_Ok(int id)
        {
            Book book = new Book();
            _mockUnitOfWork.Setup(x => x.BookRepository.GetBookByIdAsync(id)).Returns(Task.FromResult(book));
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookValidator.Object);
            Book actualBook = await _bookService.GetBookByIdAsync(id);
            
            Assert.AreEqual(book, actualBook);
            _mockUnitOfWork.Verify(x => x.BookRepository.GetBookByIdAsync(id), Times.Once);
        }

        [TestMethod]
        [DataRow("  ", "  ")]
        [DataRow("s", null)]
        [DataRow("", "s")]
        [ExpectedException(typeof(OLInternalServerError))]
        public async Task Create_Book_FieldsIsIncorrect(string name, string description)
        {
            _mockBookRepository.Setup(x => x.InsertBook(It.IsAny<Book>()));
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookValidator.Object);
            Book book = new Book() { Name = name, Description = description};
            await  _bookService.CreateBookAsync(book);
            _mockUnitOfWork.Verify(x => x.BookRepository.InsertBook(It.IsAny<Book>()), Times.Once);
        }

        [TestMethod]
        [DataRow(" s ", " s ")]
        public async Task Create_Book_Ok(string name, string description)
        {
            _mockUnitOfWork.Setup(x => x.BookRepository.InsertBook(It.IsAny<Book>()));
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookValidator.Object);
            Book book = new Book() { Id = 1, Name = name, Description = description, Authors = new List<Author>() { new Author() }, Tags = new List<Tag>() { new Tag() }, Genre = Genre.Adventures };
            await _bookService.CreateBookAsync(book);
            _mockUnitOfWork.Verify(x => x.BookRepository.InsertBook(It.IsAny<Book>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveAsync(), Times.Once);

        }
    }
}
