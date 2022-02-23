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
using OnlineLibrary.Common.Exceptions;
using System.Collections.Generic;
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

        private CreateBookValidator _bookValidator = new CreateBookValidator();

        private Mock<ILogger<BookController>> _mockILogger = new Mock<ILogger<BookController>>();

        // Task<IActionResult> FilterBookAsync([FromBody] BookProcessing bookProcessing)

        [TestMethod]
        public async Task Filter_Book_Ok()
        {
            BookProcessing bookProcessing = new BookProcessing();
            _mockBookService.Setup(x => x.FilterSortPaginBooksAsync(bookProcessing)).Returns(Task.FromResult(new PaginatedList<Book>() { }));
            _bookController = new BookController(_mockBookService.Object, _mockAuthorService.Object, _mockTagService.Object, _mockMapper.Object, _mockILogger.Object);

            var result = await _bookController.FilterBookAsync(bookProcessing);
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            _mockBookService.Verify(x => x.FilterSortPaginBooksAsync(bookProcessing), Times.Once);
        }

        // Task<IActionResult> GetBookByIdAsync(int? id)

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        public async Task Get_BookById_Ok(int bookId)
        {
            _mockBookService.Setup(x => x.GetBookByIdAsync((int)bookId)).Returns(Task.FromResult(new Book()));
            _bookController = new BookController(_mockBookService.Object, _mockAuthorService.Object, _mockTagService.Object, _mockMapper.Object, _mockILogger.Object);

            var result = await _bookController.GetBookByIdAsync(bookId);
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            _mockBookService.Verify(x => x.GetBookByIdAsync((int)bookId), Times.Once);
        }

        // Task<IActionResult> CreateAsync([FromBody] CreateBook cBook)

        [TestMethod]
        public async Task Create_Book_Ok()
        {
            _mockTagService.Setup(x => x.GetTagsByIdListAsync(It.IsAny<List<int>>())).Returns(Task.FromResult(new List<Tag>()));
            _mockAuthorService.Setup(x => x.GetAuthorsByIdListAsync(It.IsAny<List<int>>())).Returns(Task.FromResult(new List<Author>()));
            _mockMapper.Setup(x => x.Map<CreateBook, Book>(It.IsAny<CreateBook>())).Returns(new Book());
            _mockBookService.Setup(x => x.CreateBookAsync(It.IsAny<Book>())).Returns(Task.FromResult(1));
            _bookController = new BookController(_mockBookService.Object, _mockAuthorService.Object, _mockTagService.Object, _mockMapper.Object, _mockILogger.Object);
            
            var result = await _bookController.CreateAsync(new CreateBook());
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            _mockTagService.Verify(x => x.GetTagsByIdListAsync(It.IsAny<List<int>>()), Times.Once);
            _mockAuthorService.Verify(x => x.GetAuthorsByIdListAsync(It.IsAny<List<int>>()), Times.Once);
            _mockMapper.Verify(x => x.Map<CreateBook, Book>(It.IsAny<CreateBook>()), Times.Once);
            _mockBookService.Verify(x => x.CreateBookAsync(It.IsAny<Book>()), Times.Once);
        }

        // Task<IActionResult> UpdatePatchAsync(int Id, [FromBody] JsonPatchDocument<Book> book)

        //[TestMethod]
        //public async Task UpdatePatch_Book()
        //{
        //    _mockBookService.Setup(x => x.UpdatePatchAsync(It.IsAny<int>(), It.IsAny<JsonPatchDocument<Book>>()));
        //    _bookController = new BookController(_mockBookService.Object, _mockAuthorService.Object, _mockTagService.Object, _mockMapper.Object, _mockILogger.Object);
        //    await _bookController.UpdatePatchAsync(It.IsAny<int>(), It.IsAny<JsonPatchDocument<Book>>());
        //    _mockBookService.Verify(x => x.UpdatePatchAsync(It.IsAny<int>(), It.IsAny<JsonPatchDocument<Book>>()), Times.Once);
        //    _mockBookService.Verify(x => x.GetBookByIdAsync(It.IsAny<int>()), Times.Once);
        //}

        // Validate CreateBook - it passed as [FromBody] in CreateAsync method

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
    }
}
