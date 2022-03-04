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
using OnlineLibrary.DAL.DTO;
using AutoMapper;
using System;

namespace OnlineLibraryApiTest.Services
{
    [TestClass]
    public class BookServiceTest
    {
        private BookService _bookService;

        private Mock<IUnitOfWork> _mockUnitOfWork = new Mock<IUnitOfWork>();

        private Mock<IValidator<BookDTO>> _mockBookDTOValidator = new Mock<IValidator<BookDTO>>();

        private Mock<IValidator<Reservation>> _mockReservationValidator = new Mock<IValidator<Reservation>>();

        private Mock<IMapper> _mockMapper = new Mock<IMapper>();

        private Mock<IBookRepository> _mockBookRepository = new Mock<IBookRepository>();

        private Mock<IAuthorRepository> _mockAuthorRepository = new Mock<IAuthorRepository>();

        [TestInitialize]
        public void InitializeTest()
        {
            _mockUnitOfWork.Setup(x => x.BookRepository).Returns(_mockBookRepository.Object);
            _mockUnitOfWork.Setup(x => x.AuthorRepository).Returns(_mockAuthorRepository.Object);
        }

        // Task<PaginatedList<Book>> FilterBooksAsync(BookProcessing bookProcessing)

        [TestMethod]
        public async Task Filter_Books_NotFound()
        {
            _mockUnitOfWork.Setup(x => x.BookRepository.FilterSortPaginBooksAsync(It.IsAny<BookProcessing>())).Returns(Task.FromResult(new PaginatedList<Book>()));
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookDTOValidator.Object, _mockReservationValidator.Object, _mockMapper.Object);
            await Assert.ThrowsExceptionAsync<OLNotFound>(() => _bookService.FilterSortPaginBooksAsync(new BookProcessing()));
            _mockUnitOfWork.Verify(x => x.BookRepository.FilterSortPaginBooksAsync(It.IsAny<BookProcessing>()), Times.Once);
        }


        [TestMethod]
        public async Task Filter_Books_Ok()
        {
            _mockUnitOfWork.Setup(x => x.BookRepository.FilterSortPaginBooksAsync(It.IsAny<BookProcessing>())).Returns(Task.FromResult(new PaginatedList<Book>(1, new List<Book>() { new Book() })));
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookDTOValidator.Object, _mockReservationValidator.Object, _mockMapper.Object);
            await _bookService.FilterSortPaginBooksAsync(new BookProcessing());
            _mockUnitOfWork.Verify(x => x.BookRepository.FilterSortPaginBooksAsync(It.IsAny<BookProcessing>()), Times.Once);
        }

        // Task<Book> GetBookByIdAsync(int? bookId)

        [TestMethod]
        public async Task Get_BookById_NotFound()
        {
            Book book = null;
            _mockUnitOfWork.Setup(x => x.BookRepository.GetBookByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(book));
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookDTOValidator.Object, _mockReservationValidator.Object, _mockMapper.Object);

            await Assert.ThrowsExceptionAsync<OLNotFound>(() => _bookService.GetBookByIdAsync(1));
            _mockUnitOfWork.Verify(x => x.BookRepository.GetBookByIdAsync(It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        public async Task Get_BookById_Ok()
        {
            _mockUnitOfWork.Setup(x => x.BookRepository.GetBookByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(new Book()));
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookDTOValidator.Object, _mockReservationValidator.Object, _mockMapper.Object);

            await _bookService.GetBookByIdAsync(1);

            _mockUnitOfWork.Verify(x => x.BookRepository.GetBookByIdAsync(It.IsAny<int>()), Times.Once);
        }

        // Task UpdatePatchAsync(int bookId, JsonPatchDocument<Book> book)

        [TestMethod]
        public async Task UpdatePatch_Book_OLNotFound()
        {
            Book bookNull = null;
            _mockUnitOfWork.Setup(x => x.BookRepository.GetBookByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(bookNull));
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookDTOValidator.Object, _mockReservationValidator.Object, _mockMapper.Object);

            await Assert.ThrowsExceptionAsync<OLNotFound>(() => _bookService.UpdatePatchAsync(1, new JsonPatchDocument<BookDTO>()));

            _mockMapper.Verify(x => x.Map<Book, BookDTO>(It.IsAny<Book>()), Times.Never);
            _mockUnitOfWork.Verify(x => x.BookRepository.UpdateBookAsync(It.IsAny<BookDTO>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdatePatch_Book_TryToUpdateId_BadRequest()
        {
            JsonPatchDocument<BookDTO> book = new JsonPatchDocument<BookDTO>();
            book.Operations.Add(new Operation<BookDTO>("replace", "/Id", "/books/1", 2));
            _mockMapper.Setup(x => x.Map<Book, BookDTO>(It.IsAny<Book>())).Returns(new BookDTO() { Id = 1});
            _mockUnitOfWork.Setup(x => x.BookRepository.GetBookByIdAsync(1)).Returns(Task.FromResult(new Book() { Id = 1 }));
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookDTOValidator.Object, _mockReservationValidator.Object, _mockMapper.Object);

            await Assert.ThrowsExceptionAsync<OLBadRequest>(() => _bookService.UpdatePatchAsync(1, book));
            _mockMapper.Verify(x => x.Map<Book, BookDTO>(It.IsAny<Book>()), Times.Once);
            _mockBookDTOValidator.Verify(x => x.Validate(It.IsAny<BookDTO>()), Times.Never);
            _mockUnitOfWork.Verify(x => x.BookRepository.UpdateBookAsync(It.IsAny<BookDTO>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdatePatch_Book_ValidationFalied_BadRequest()
        {
            JsonPatchDocument<BookDTO> book = new JsonPatchDocument<BookDTO>();
            book.Operations.Add(new Operation<BookDTO>("replace", "/tags", "/books/1", new List<Tag>()));
            _mockMapper.Setup(x => x.Map<Book, BookDTO>(It.IsAny<Book>())).Returns(new BookDTO() { Id = 1});
            _mockBookDTOValidator.Setup(x => x.Validate(It.IsAny<BookDTO>())).Returns(new ValidationResult(new List<ValidationFailure>() { new ValidationFailure("tags", "error") }));
            _mockUnitOfWork.Setup(x => x.BookRepository.GetBookByIdAsync(1)).Returns(Task.FromResult(new Book() { Id = 1 }));
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookDTOValidator.Object, _mockReservationValidator.Object, _mockMapper.Object);

            await Assert.ThrowsExceptionAsync<OLBadRequest>(() => _bookService.UpdatePatchAsync(1, book));
            _mockBookDTOValidator.Verify(x => x.Validate(It.IsAny<BookDTO>()), Times.Once);
            _mockMapper.Verify(x => x.Map<Book, BookDTO>(It.IsAny<Book>()), Times.Once);
            _mockReservationValidator.Verify(x => x.Validate(It.IsAny<Reservation>()), Times.Never);
            _mockUnitOfWork.Verify(x => x.BookRepository.UpdateBookAsync(It.IsAny<BookDTO>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdatePatch_Reservation_ValidationFalied_BookIdIsIncorrect_BadRequest()
        {
            JsonPatchDocument<BookDTO> book = new JsonPatchDocument<BookDTO>();
            List<Reservation> reservation = new List<Reservation>() { new Reservation() { BookId = 2 } };
            book.Operations.Add(new Operation<BookDTO>("replace", "/reservations", "/books/1", reservation));
            _mockMapper.Setup(x => x.Map<Book, BookDTO>(It.IsAny<Book>())).Returns(new BookDTO() { Id = 1 });
            _mockBookDTOValidator.Setup(x => x.Validate(It.IsAny<BookDTO>())).Returns(new ValidationResult(new List<ValidationFailure>()));
            _mockUnitOfWork.Setup(x => x.BookRepository.GetBookByIdAsync(1)).Returns(Task.FromResult(new Book() { Id = 1 }));
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookDTOValidator.Object, _mockReservationValidator.Object, _mockMapper.Object);

            await Assert.ThrowsExceptionAsync<OLBadRequest>(() => _bookService.UpdatePatchAsync(1, book));
            _mockBookDTOValidator.Verify(x => x.Validate(It.IsAny<BookDTO>()), Times.Once);
            _mockMapper.Verify(x => x.Map<Book, BookDTO>(It.IsAny<Book>()), Times.Once);
            _mockReservationValidator.Verify(x => x.Validate(It.IsAny<Reservation>()), Times.Never);
            _mockUnitOfWork.Verify(x => x.BookRepository.UpdateBookAsync(It.IsAny<BookDTO>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdatePatch_Reservation_ValidationFalied_MoreThanOneReturnDateIsNull_BadRequest()
        {
            JsonPatchDocument<BookDTO> book = new JsonPatchDocument<BookDTO>();
            List<Reservation> reservation = new List<Reservation>() { new Reservation() { BookId = 1, ReturnDate = null }, new Reservation() { BookId = 1, ReturnDate = null } };
            book.Operations.Add(new Operation<BookDTO>("replace", "/reservations", "/books/1", reservation));
            _mockMapper.Setup(x => x.Map<Book, BookDTO>(It.IsAny<Book>())).Returns(new BookDTO() { Id = 1 });
            _mockBookDTOValidator.Setup(x => x.Validate(It.IsAny<BookDTO>())).Returns(new ValidationResult(new List<ValidationFailure>()));
            _mockUnitOfWork.Setup(x => x.BookRepository.GetBookByIdAsync(1)).Returns(Task.FromResult(new Book() { Id = 1 }));
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookDTOValidator.Object, _mockReservationValidator.Object, _mockMapper.Object);

            await Assert.ThrowsExceptionAsync<OLBadRequest>(() => _bookService.UpdatePatchAsync(1, book));
            _mockBookDTOValidator.Verify(x => x.Validate(It.IsAny<BookDTO>()), Times.Once);
            _mockMapper.Verify(x => x.Map<Book, BookDTO>(It.IsAny<Book>()), Times.Once);
            _mockReservationValidator.Verify(x => x.Validate(It.IsAny<Reservation>()), Times.Never);
            _mockUnitOfWork.Verify(x => x.BookRepository.UpdateBookAsync(It.IsAny<BookDTO>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdatePatch_Reservation_ValidationFalied_ReservationIsInvalid_BadRequest()
        {
            JsonPatchDocument<BookDTO> book = new JsonPatchDocument<BookDTO>();
            List<Reservation> reservation = new List<Reservation>() { new Reservation() { BookId = 1, UserId = 0 } };
            book.Operations.Add(new Operation<BookDTO>("replace", "/reservations", "/books/1", reservation));
            _mockMapper.Setup(x => x.Map<Book, BookDTO>(It.IsAny<Book>())).Returns(new BookDTO() { Id = 1 });
            _mockBookDTOValidator.Setup(x => x.Validate(It.IsAny<BookDTO>())).Returns(new ValidationResult(new List<ValidationFailure>()));
            _mockReservationValidator.Setup(x => x.Validate(It.IsAny<Reservation>())).Returns(new ValidationResult(new List<ValidationFailure>() { new ValidationFailure("reservations", "error") }));
            _mockUnitOfWork.Setup(x => x.BookRepository.GetBookByIdAsync(1)).Returns(Task.FromResult(new Book() { Id = 1 }));
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookDTOValidator.Object, _mockReservationValidator.Object, _mockMapper.Object);

            await Assert.ThrowsExceptionAsync<OLBadRequest>(() => _bookService.UpdatePatchAsync(1, book));
            _mockBookDTOValidator.Verify(x => x.Validate(It.IsAny<BookDTO>()), Times.Once);
            _mockMapper.Verify(x => x.Map<Book, BookDTO>(It.IsAny<Book>()), Times.Once);
            _mockReservationValidator.Verify(x => x.Validate(It.IsAny<Reservation>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.BookRepository.UpdateBookAsync(It.IsAny<BookDTO>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdatePatch_Reservation_ValidationFalied_ReservationsOverlap_BadRequest()
        {
            JsonPatchDocument<BookDTO> book = new JsonPatchDocument<BookDTO>();
            List<Reservation> reservation = new List<Reservation>() { 
                new Reservation() { BookId = 1, UserId = 1, ReservationDate = new DateTime(2020, 1, 1), ReturnDate = new DateTime(2021, 1, 1)},
                new Reservation() { BookId = 1, UserId = 1, ReservationDate = new DateTime(2020, 12, 1), ReturnDate = new DateTime(2021, 12, 1) }
            };
            book.Operations.Add(new Operation<BookDTO>("replace", "/reservations", "/books/1", reservation));
            _mockMapper.Setup(x => x.Map<Book, BookDTO>(It.IsAny<Book>())).Returns(new BookDTO() { Id = 1 });
            _mockBookDTOValidator.Setup(x => x.Validate(It.IsAny<BookDTO>())).Returns(new ValidationResult(new List<ValidationFailure>()));
            _mockReservationValidator.Setup(x => x.Validate(It.IsAny<Reservation>())).Returns(new ValidationResult(new List<ValidationFailure>()));
            _mockUnitOfWork.Setup(x => x.BookRepository.GetBookByIdAsync(1)).Returns(Task.FromResult(new Book() { Id = 1 }));
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookDTOValidator.Object, _mockReservationValidator.Object, _mockMapper.Object);

            await Assert.ThrowsExceptionAsync<OLBadRequest>(() => _bookService.UpdatePatchAsync(1, book));
            _mockBookDTOValidator.Verify(x => x.Validate(It.IsAny<BookDTO>()), Times.Once);
            _mockMapper.Verify(x => x.Map<Book, BookDTO>(It.IsAny<Book>()), Times.Once);
            _mockReservationValidator.Verify(x => x.Validate(It.IsAny<Reservation>()), Times.Exactly(2));
            _mockUnitOfWork.Verify(x => x.BookRepository.UpdateBookAsync(It.IsAny<BookDTO>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdatePatch_Book_ValidationFalied_ArchivationWhenReservationIsOpen_BadRequest()
        {
            JsonPatchDocument<BookDTO> book = new JsonPatchDocument<BookDTO>();
            List<Reservation> reservation = new List<Reservation>() {
                new Reservation() { BookId = 1, UserId = 1, ReservationDate = new DateTime(2020, 1, 1), ReturnDate = new DateTime(2021, 1, 1)},
                new Reservation() { BookId = 1, UserId = 1, ReservationDate = new DateTime(2021, 12, 1), ReturnDate = null }
            };
            book.Operations.Add(new Operation<BookDTO>("replace", "/reservations", "/books/1", reservation));
            book.Operations.Add(new Operation<BookDTO>("replace", "/InArchive", "/books/1", true));
            
            _mockMapper.Setup(x => x.Map<Book, BookDTO>(It.IsAny<Book>())).Returns(new BookDTO() { Id = 1 });
            _mockBookDTOValidator.Setup(x => x.Validate(It.IsAny<BookDTO>())).Returns(new ValidationResult(new List<ValidationFailure>()));
            _mockReservationValidator.Setup(x => x.Validate(It.IsAny<Reservation>())).Returns(new ValidationResult(new List<ValidationFailure>()));
            _mockUnitOfWork.Setup(x => x.BookRepository.GetBookByIdAsync(1)).Returns(Task.FromResult(new Book() { Id = 1 }));
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookDTOValidator.Object, _mockReservationValidator.Object, _mockMapper.Object);

            await Assert.ThrowsExceptionAsync<OLBadRequest>(() => _bookService.UpdatePatchAsync(1, book));
            _mockBookDTOValidator.Verify(x => x.Validate(It.IsAny<BookDTO>()), Times.Once);
            _mockMapper.Verify(x => x.Map<Book, BookDTO>(It.IsAny<Book>()), Times.Once);
            _mockReservationValidator.Verify(x => x.Validate(It.IsAny<Reservation>()), Times.Exactly(2));
            _mockUnitOfWork.Verify(x => x.BookRepository.UpdateBookAsync(It.IsAny<BookDTO>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdatePatch_Book_Ok()
        {
            JsonPatchDocument<BookDTO> book = new JsonPatchDocument<BookDTO>();
            List<Reservation> reservation = new List<Reservation>() {
                new Reservation() { BookId = 1, UserId = 1, ReservationDate = new DateTime(2020, 1, 1), ReturnDate = new DateTime(2021, 1, 1)},
                new Reservation() { BookId = 1, UserId = 1, ReservationDate = new DateTime(2021, 12, 1), ReturnDate = null }
            };
            book.Operations.Add(new Operation<BookDTO>("replace", "/reservations", "/books/1", reservation));
            book.Operations.Add(new Operation<BookDTO>("replace", "/InArchive", "/books/1", false));

            _mockMapper.Setup(x => x.Map<Book, BookDTO>(It.IsAny<Book>())).Returns(new BookDTO() { Id = 1 });
            _mockBookDTOValidator.Setup(x => x.Validate(It.IsAny<BookDTO>())).Returns(new ValidationResult(new List<ValidationFailure>()));
            _mockReservationValidator.Setup(x => x.Validate(It.IsAny<Reservation>())).Returns(new ValidationResult(new List<ValidationFailure>()));
            _mockUnitOfWork.Setup(x => x.BookRepository.GetBookByIdAsync(1)).Returns(Task.FromResult(new Book() { Id = 1 }));
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookDTOValidator.Object, _mockReservationValidator.Object, _mockMapper.Object);

            await  _bookService.UpdatePatchAsync(1, book);
            _mockUnitOfWork.Verify(x => x.BookRepository.UpdateBookAsync(It.IsAny<BookDTO>()), Times.Once);
        }

        // Task<int> CreateBookAsync(Book book)

        [TestMethod]
        public async Task Create_Book_OLInternalServerError()
        {
            _mockBookRepository.Setup(x => x.CreateBookAsync(It.IsAny<Book>()));
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookDTOValidator.Object, _mockReservationValidator.Object, _mockMapper.Object);
            await Assert.ThrowsExceptionAsync<OLInternalServerError>(() => _bookService.CreateBookAsync(new Book()));
            _mockUnitOfWork.Verify(x => x.BookRepository.CreateBookAsync(It.IsAny<Book>()), Times.Once);
        }

        [TestMethod]
        public async Task Create_Book_Ok()
        {
            _mockBookRepository.Setup(x => x.CreateBookAsync(It.IsAny<Book>()));
            _bookService = new BookService(_mockUnitOfWork.Object, _mockBookDTOValidator.Object, _mockReservationValidator.Object, _mockMapper.Object);
            await _bookService.CreateBookAsync(new Book() { Id = 1});
            _mockUnitOfWork.Verify(x => x.BookRepository.CreateBookAsync(It.IsAny<Book>()), Times.Once);
        }
    }
}
