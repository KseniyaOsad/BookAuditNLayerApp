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
using System.Collections.Generic;
using OnlineLibrary.Common.DBEntities.Enums;
using OnlineLibrary.Common.EntityProcessing;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch.Operations;
using FluentValidation.Results;

namespace OnlineLibraryApiTest.Services
{
    [TestClass]
    public class BookServiceTest
    {
        private BookService _bookService;

        private Mock<IUnitOfWork> _mockUnitOfWork = new Mock<IUnitOfWork>();

        private Mock<IValidator<Book>> _mockBookValidator = new Mock<IValidator<Book>>();

        private BookValidator _bookValidator = new BookValidator();

        private Mock<IBookRepository> _mockBookRepository = new Mock<IBookRepository>();

        private Mock<IAuthorRepository> _mockAuthorRepository = new Mock<IAuthorRepository>();

        [TestInitialize]
        public void InitializeTest()
        {
            _mockUnitOfWork.Setup(x => x.BookRepository).Returns(_mockBookRepository.Object);
            _mockUnitOfWork.Setup(x => x.AuthorRepository).Returns(_mockAuthorRepository.Object);
        }

        // Task<PaginatedList<Book>> FilterBooksAsync(BookProcessing bookProcessing)

        //[TestMethod]
        //public async Task Filter_Books_Ok()
        //{
        //    _mockUnitOfWork.Setup(x => x.BookRepository.FilterBooksAsync(It.IsAny<BookProcessing>())).Returns(Task.FromResult(new PaginatedList<Book>()));
        //    _bookService = new BookService(_mockUnitOfWork.Object, _mockBookValidator.Object);

        //    await _bookService.FilterSortPaginBooksAsync(new BookProcessing());
        //    _mockUnitOfWork.Verify(x => x.BookRepository.FilterBooksAsync(It.IsAny<BookProcessing>()), Times.Once);
        //}


        // Task<Book> GetBookByIdAsync(int? bookId)

        [TestMethod]
        public async Task Get_BookById_NotFound()
        {
            Book book = null;
            _mockUnitOfWork.Setup(x => x.BookRepository.GetBookByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(book));
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookValidator.Object);

            await Assert.ThrowsExceptionAsync<OLNotFound>(() => _bookService.GetBookByIdAsync(1));
            _mockUnitOfWork.Verify(x => x.BookRepository.GetBookByIdAsync(It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        public async Task Get_BookById_Ok()
        {
            _mockUnitOfWork.Setup(x => x.BookRepository.GetBookByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(new Book()));
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookValidator.Object);
           
            await _bookService.GetBookByIdAsync(1);

            _mockUnitOfWork.Verify(x => x.BookRepository.GetBookByIdAsync(It.IsAny<int>()), Times.Once);
        }

        // Task UpdatePatchAsync(int bookId, JsonPatchDocument<Book> book)
        
        [TestMethod]
        public async Task UpdatePatch_Book_OLNotFound()
        {
            Book bookNull = null;
            _mockBookValidator.Setup(x => x.Validate(It.IsAny<Book>()));
            _mockUnitOfWork.Setup(x => x.BookRepository.GetBookByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(bookNull));
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookValidator.Object);
            
            await Assert.ThrowsExceptionAsync<OLNotFound>(() => _bookService.UpdatePatchAsync(1, new JsonPatchDocument<Book>()));

            _mockBookValidator.Verify(x => x.Validate(It.IsAny<Book>()), Times.Never);
           // _mockUnitOfWork.Verify(x => x.BookRepository.UpdateBookAsync(It.IsAny<Book>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdatePatch_Book_TryToUpdateId_BadRequest()
        {
            JsonPatchDocument<Book> book = new JsonPatchDocument<Book>();
            book.Operations.Add(new Operation<Book>("replace", "/Id", "/books/1", 2));
            _mockBookValidator.Setup(x => x.Validate(It.IsAny<Book>()));
            _mockUnitOfWork.Setup(x => x.BookRepository.GetBookByIdAsync(1)).Returns(Task.FromResult(new Book() { Id = 1 }));
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookValidator.Object);
            
            await Assert.ThrowsExceptionAsync<OLBadRequest>(() => _bookService.UpdatePatchAsync(1, book));
            _mockBookValidator.Verify(x => x.Validate(It.IsAny<Book>()), Times.Never);
            //_mockUnitOfWork.Verify(x => x.BookRepository.UpdateBookAsync(It.IsAny<Book>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdatePatch_Book_TryToUpdateUnExistAuthors_BadRequest()
        {
            JsonPatchDocument<Book> book = new JsonPatchDocument<Book>();
            book.Operations.Add(new Operation<Book>("replace", "/authors", "/books/1", new List<Author>() { new Author() { Id = 1 } }));
            _mockBookValidator.Setup(x => x.Validate(It.IsAny<Book>())).Returns(new ValidationResult());
            _mockUnitOfWork.Setup(x => x.BookRepository.GetBookByIdAsync(1)).Returns(Task.FromResult(new Book() { Id = 1 }));
            _mockUnitOfWork.Setup(x => x.AuthorRepository.GetAuthorsByIdListAsync(It.IsAny<List<int>>())).Returns(Task.FromResult(new List<Author>()));
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookValidator.Object);
            
            await Assert.ThrowsExceptionAsync<OLBadRequest>(() => _bookService.UpdatePatchAsync(1, book));
            
            _mockBookValidator.Verify(x => x.Validate(It.IsAny<Book>()), Times.Once);
            //_mockUnitOfWork.Verify(x => x.BookRepository.UpdateBookAsync(It.IsAny<Book>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdatePatch_Book_TryToUpdateUnExistTags_BadRequest()
        {
            JsonPatchDocument<Book> book = new JsonPatchDocument<Book>();
            book.Operations.Add(new Operation<Book>("replace", "/tags", "/books/1", new List<Tag>() { new Tag() { Id = 1 } }));
            _mockBookValidator.Setup(x => x.Validate(It.IsAny<Book>())).Returns(new ValidationResult());
            _mockUnitOfWork.Setup(x => x.BookRepository.GetBookByIdAsync(1)).Returns(Task.FromResult(new Book() { Id = 1 }));
            _mockUnitOfWork.Setup(x => x.TagRepository.GetTagsByIdListAsync(It.IsAny<List<int>>())).Returns(Task.FromResult(new List<Tag>()));
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookValidator.Object);
            
            await Assert.ThrowsExceptionAsync<OLBadRequest>(() => _bookService.UpdatePatchAsync(1, book));
            _mockBookValidator.Verify(x => x.Validate(It.IsAny<Book>()), Times.Once);
            //_mockUnitOfWork.Verify(x => x.BookRepository.UpdateBookAsync(It.IsAny<Book>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdatePatch_Book_ValidationFalied_BadRequest()
        {
            JsonPatchDocument<Book> book = new JsonPatchDocument<Book>();
            book.Operations.Add(new Operation<Book>("replace", "/tags", "/books/1", new List<Tag>() ));
            _mockBookValidator.Setup(x => x.Validate(It.IsAny<Book>())).Returns(new ValidationResult(new List<ValidationFailure>() { new ValidationFailure("tags", "error") }));
            _mockUnitOfWork.Setup(x => x.BookRepository.GetBookByIdAsync(1)).Returns(Task.FromResult(new Book() { Id = 1 }));
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookValidator.Object);
            
            await Assert.ThrowsExceptionAsync<OLBadRequest>(() => _bookService.UpdatePatchAsync(1, book));
            _mockBookValidator.Verify(x => x.Validate(It.IsAny<Book>()), Times.Once);
            //_mockUnitOfWork.Verify(x => x.BookRepository.UpdateBookAsync(It.IsAny<Book>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdatePatch_Book_Ok()
        {
            JsonPatchDocument<Book> book = new JsonPatchDocument<Book>();
            book.Operations.Add(new Operation<Book>("replace", "/name", "/books/1", "Alice"));
            _mockBookValidator.Setup(x => x.Validate(It.IsAny<Book>())).Returns(new ValidationResult()); ;
            Book originalBook = new Book() { Id = 1, Name = "NotAlice" };
            _mockUnitOfWork.Setup(x => x.BookRepository.GetBookByIdAsync(1)).Returns(Task.FromResult(originalBook));
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookValidator.Object);
            await _bookService.UpdatePatchAsync(1, book);
            Assert.AreEqual(originalBook.Name, "Alice");
            _mockBookValidator.Verify(x => x.Validate(It.IsAny<Book>()), Times.Once);
            //_mockUnitOfWork.Verify(x => x.BookRepository.UpdateBookAsync(It.IsAny<Book>(), true, It.IsAny<bool>(), It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public async Task UpdatePatch_Archivate_Book_BookInReserve()
        {
            JsonPatchDocument<Book> book = new JsonPatchDocument<Book>();
            book.Operations.Add(new Operation<Book>("replace", "/inArchive", "/books/1", "true"));
            _mockBookValidator.Setup(x => x.Validate(It.IsAny<Book>())).Returns(new ValidationResult()); ;
            Book originalBook = new Book() { Id = 1, Name = "Alice" };
            _mockUnitOfWork.Setup(x => x.BookRepository.GetBookByIdAsync(1)).Returns(Task.FromResult(originalBook));
            _mockUnitOfWork.Setup(x => x.ReservationRepository.GetBookReservationLastRow(It.IsAny<int>())).Returns(Task.FromResult(new Reservation() { ReturnDate = default}));
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookValidator.Object);
           
            await Assert.ThrowsExceptionAsync<OLBadRequest>(() => _bookService.UpdatePatchAsync(1, book));

            _mockBookValidator.Verify(x => x.Validate(It.IsAny<Book>()), Times.Once);
            //_mockUnitOfWork.Verify(x => x.BookRepository.UpdateBookAsync(It.IsAny<Book>(), true, It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);

        }

        // Task<int> CreateBookAsync(Book book)

        [TestMethod]
        public async Task Create_Book_OLInternalServerError()
        {
            _mockBookRepository.Setup(x => x.CreateBookAsync(It.IsAny<Book>()));
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookValidator.Object);
            await Assert.ThrowsExceptionAsync<OLInternalServerError>(() => _bookService.CreateBookAsync(new Book()));
            _mockUnitOfWork.Verify(x => x.BookRepository.CreateBookAsync(It.IsAny<Book>()), Times.Once);
        }

        [TestMethod]
        public async Task Create_Book_Ok()
        {
            _mockBookRepository.Setup(x => x.CreateBookAsync(It.IsAny<Book>()));
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookValidator.Object);
            await _bookService.CreateBookAsync(new Book() { Id = 1});
            _mockUnitOfWork.Verify(x => x.BookRepository.CreateBookAsync(It.IsAny<Book>()), Times.Once);
        }

        // Validate Book - it used in UpdatePatchAsync method

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
    }
}
