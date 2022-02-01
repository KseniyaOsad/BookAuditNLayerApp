using AutoMapper;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OnlineLibrary.API.Controllers;
using OnlineLibrary.API.Model;
using OnlineLibrary.API.Validator;
using OnlineLibrary.BLL.Interfaces;
using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.Common.DBEntities.Enums;
using OnlineLibrary.Common.EntityProcessing;
using OnlineLibrary.Common.EntityProcessing.Pagination;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLibraryApiTest.Controllers
{
    [TestClass]
    public class BookControllerTest
    {
        private BookController _bookController;

        private Mock<IBookService> _mockBookService = new Mock<IBookService>();

        private Mock<IAuthorService> _mockAuthorService = new Mock<IAuthorService>();

        private Mock<ITagService> _mockTagService = new Mock<ITagService>();

        private Mock<IMapper> _mockMapper = new Mock<IMapper>();

        private CreateBookValidator _bookValidator;

        private Mock<ILogger<BookController>> _mockILogger = new Mock<ILogger<BookController>>();

        [TestInitialize]
        public void InitializeTest()
        {
            _bookValidator = new CreateBookValidator();
        }

        [TestMethod]
        public async Task UpdatePatch_Book()
        {
            _mockBookService.Setup(x => x.UpdatePatchAsync(It.IsAny<int>(), It.IsAny<JsonPatchDocument<Book>>()));
            _bookController = new BookController(_mockBookService.Object, _mockAuthorService.Object, _mockTagService.Object, _mockMapper.Object, _mockILogger.Object);
            await _bookController.UpdatePatchAsync(It.IsAny<int>(), It.IsAny<JsonPatchDocument<Book>>());
            _mockBookService.Verify(x => x.UpdatePatchAsync(It.IsAny<int>(), It.IsAny<JsonPatchDocument<Book>>()), Times.Once);
            _mockBookService.Verify(x => x.GetBookByIdAsync(It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        [DataRow(null, "   ", -2)]
        [DataRow("", null, 0)]
        [DataRow("  ", null, 90)]
        public void Validate_CreateBook_FieldIsIncorrect(string name, string descr, Genre genre)
        {
            CreateBook book = new CreateBook() { Name = name, Description = descr, Genre = genre, Authors = new List<int> { 2} };
            var result = _bookValidator.TestValidate(book);
            result.ShouldHaveValidationErrorFor(x => x.Name);
            result.ShouldHaveValidationErrorFor(x => x.Description);
            result.ShouldHaveValidationErrorFor(x => x.Genre);
        }

        [TestMethod]
        [DataRow("", "s", 3, new int[] { 1, 2 })]
        [DataRow("s", "", 2, new int[] { 4 })]
        [DataRow(null, "s", 1, new int[] { })]
        [DataRow("s", "s", 1, new int[] { })]
        public async Task Create_Book(string name, string descr, Genre genre, int[] authors)
        {
            List<int> ints = authors.ToList();
            CreateBook cBook = new CreateBook() { Name = name, Description = descr, Genre = genre, Authors = ints };

            _mockAuthorService.Setup(x => x.GetAuthorsByIdListAsync(ints));
            _mockBookService.Setup(x => x.CreateBookAsync(It.IsAny<Book>()));
            _mockMapper.Setup(x => x.Map<CreateBook, Book>(It.IsAny<CreateBook>())).Returns(new Book());
            _bookController = new BookController(_mockBookService.Object, _mockAuthorService.Object, _mockTagService.Object, _mockMapper.Object, _mockILogger.Object);

            await _bookController.CreateAsync(cBook);
            _mockAuthorService.Verify(x => x.GetAuthorsByIdListAsync(ints), Times.Once);
            _mockBookService.Verify(x => x.CreateBookAsync(It.IsAny<Book>()), Times.Once);
        }

        [TestMethod]
        public async Task Filter_Book_Ok()
        {
            BookProcessing bookProcessing = new BookProcessing();
            _mockBookService.Setup(x => x.FilterBooksAsync(bookProcessing)).Returns(Task.FromResult(new PaginatedList<Book>() { }));
            _bookController = new BookController(_mockBookService.Object, _mockAuthorService.Object, _mockTagService.Object, _mockMapper.Object, _mockILogger.Object);

            var result = await _bookController.FilterBookAsync(bookProcessing);
            var okResult = result as OkObjectResult;
            
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            _mockBookService.Verify(x => x.FilterBooksAsync(bookProcessing), Times.Once);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(null)]
        [DataRow(-3)]
        public async Task Get_BookById_Ok(int? bookId)
        {
            _mockBookService.Setup(x => x.GetBookByIdAsync(bookId)).Returns(Task.FromResult(new Book()));
            _bookController = new BookController(_mockBookService.Object, _mockAuthorService.Object, _mockTagService.Object, _mockMapper.Object, _mockILogger.Object);

            var result = await _bookController.GetBookByIdAsync(bookId);
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            _mockBookService.Verify(x => x.GetBookByIdAsync(bookId), Times.Once);

        }

        [TestMethod]
        [DataRow("s", "s", 1, new int[] { 1, 2 })]
        [DataRow("a", "a", 2, new int[] { 1 })]
        public async Task Create_Book_Ok(string name, string descr, Genre genre, int[] authors)
        {
            CreateBook cBook = new CreateBook() { Name = name, Description = descr, Genre = genre, Authors = authors.ToList() };
            
            _mockMapper.Setup(x => x.Map<CreateBook, Book>(It.IsAny<CreateBook>())).Returns(new Book());
            List<Author> AList = new List<Author>() { new Author() };
            _mockAuthorService.Setup(x => x.GetAuthorsByIdListAsync(It.IsAny<List<int>>())).Returns(Task.FromResult(AList));
            _mockBookService.Setup(x => x.CreateBookAsync(It.IsAny<Book>())).Returns(Task.FromResult(1));
            _bookController = new BookController(_mockBookService.Object, _mockAuthorService.Object, _mockTagService.Object, _mockMapper.Object, _mockILogger.Object);

            var result = await _bookController.CreateAsync(cBook);
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            _mockAuthorService.Verify(x => x.GetAuthorsByIdListAsync(It.IsAny<List<int>>()), Times.Once);
            _mockBookService.Verify(x => x.CreateBookAsync(It.IsAny<Book>()), Times.Once);

        }

    }
}
