using AutoMapper;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
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

        [TestInitialize]
        public void InitializeTest()
        {
            _bookValidator = new CreateBookValidator();
        }

        [TestMethod]
        public void UpdatePatch_Book()
        {
            _mockBookService.Setup(x => x.UpdatePatch(It.IsAny<int>(), It.IsAny<JsonPatchDocument<Book>>()));
            _bookController = new BookController(_mockBookService.Object, _mockAuthorService.Object, _mockTagService.Object, _mockMapper.Object);
            _bookController.UpdatePatch(It.IsAny<int>(), It.IsAny<JsonPatchDocument<Book>>());
            _mockBookService.Verify(x => x.UpdatePatch(It.IsAny<int>(), It.IsAny<JsonPatchDocument<Book>>()), Times.Once);
            _mockBookService.Verify(x => x.GetBookById(It.IsAny<int>()), Times.Once);
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
        public void Create_Book(string name, string descr, Genre genre, int[] authors)
        {
            List<int> ints = authors.ToList();
            CreateBook cBook = new CreateBook() { Name = name, Description = descr, Genre = genre, Authors = ints };

            _mockAuthorService.Setup(x => x.GetAuthorsByIdList(ints));
            _mockBookService.Setup(x => x.CreateBook(It.IsAny<Book>()));
            _mockMapper.Setup(x => x.Map<CreateBook, Book>(It.IsAny<CreateBook>())).Returns(new Book());
            _bookController = new BookController(_mockBookService.Object, _mockAuthorService.Object, _mockTagService.Object, _mockMapper.Object);

            _bookController.Create(cBook);
            _mockAuthorService.Verify(x => x.GetAuthorsByIdList(ints), Times.Once);
            _mockBookService.Verify(x => x.CreateBook(It.IsAny<Book>()), Times.Once);
        }

        [TestMethod]
        public void Get_AllBooks_WithPagination_OK()
        {
            _mockBookService.Setup(x => x.GetAllBooks(It.IsAny<PaginationOptions>())).Returns(new PaginatedList<Book>() { });
            _bookController = new BookController(_mockBookService.Object, _mockAuthorService.Object, _mockTagService.Object, _mockMapper.Object);

            var result = _bookController.GetAllBooks(It.IsAny<PaginationOptions>());
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            _mockBookService.Verify(x => x.GetAllBooks(It.IsAny<PaginationOptions>()), Times.Once);
        }

        [TestMethod]
        public void Filter_Book_Ok()
        {
            BookProcessing bookProcessing = new BookProcessing();
            _mockBookService.Setup(x => x.FilterBooks(bookProcessing)).Returns(new PaginatedList<Book>() { });
            _bookController = new BookController(_mockBookService.Object, _mockAuthorService.Object, _mockTagService.Object, _mockMapper.Object);

            var result = _bookController.FilterBook(bookProcessing);
            var okResult = result as OkObjectResult;
            
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            _mockBookService.Verify(x => x.FilterBooks(bookProcessing), Times.Once);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(null)]
        [DataRow(-3)]
        public void Get_BookById_Ok(int? bookId)
        {
            _mockBookService.Setup(x => x.GetBookById(bookId)).Returns(new Book());
            _bookController = new BookController(_mockBookService.Object, _mockAuthorService.Object, _mockTagService.Object, _mockMapper.Object);

            var result = _bookController.GetBookById(bookId);
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            _mockBookService.Verify(x => x.GetBookById(bookId), Times.Once);

        }

        [TestMethod]
        [DataRow("s", "s", 1, new int[] { 1, 2 })]
        [DataRow("a", "a", 2, new int[] { 1 })]
        public void Create_Book_Ok(string name, string descr, Genre genre, int[] authors)
        {
            CreateBook cBook = new CreateBook() { Name = name, Description = descr, Genre = genre, Authors = authors.ToList() };
            
            _mockMapper.Setup(x => x.Map<CreateBook, Book>(It.IsAny<CreateBook>())).Returns(new Book());
            _mockAuthorService.Setup(x => x.GetAuthorsByIdList(It.IsAny<List<int>>())).Returns(new List<Author>() { new Author() });
            _mockBookService.Setup(x => x.CreateBook(It.IsAny<Book>())).Returns(1);
            _bookController = new BookController(_mockBookService.Object, _mockAuthorService.Object, _mockTagService.Object, _mockMapper.Object);

            var result = _bookController.Create(cBook);
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            _mockAuthorService.Verify(x => x.GetAuthorsByIdList(It.IsAny<List<int>>()), Times.Once);
            _mockBookService.Verify(x => x.CreateBook(It.IsAny<Book>()), Times.Once);

        }

    }
}
