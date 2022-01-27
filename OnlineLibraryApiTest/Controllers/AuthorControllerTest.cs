using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OnlineLibrary.API.Controllers;
using OnlineLibrary.BLL.Interfaces;
using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.Common.Validators;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineLibraryApiTest.Controllers
{
    [TestClass]
    public class AuthorControllerTest
    {
        private AuthorController _authorController;

        private Mock<IAuthorService> _mockAuthorService = new Mock<IAuthorService>();

        private AuthorValidator _authorValidator;

        [TestInitialize]
        public void TestInitialize()
        {
            _authorValidator = new AuthorValidator();
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("  ")]
        [DataRow(null)]
        public void Validate_Author_FieldIsIncorrect(string name)
        {
            Author author = new Author() { Name = name };
            var result = _authorValidator.TestValidate(author);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [TestMethod]
        public async Task Get_AllAuthors_ListIsEmpty_Ok()
        {
            _mockAuthorService.Setup(x => x.GetAllAuthorsAsync()).Returns(Task.FromResult(new List<Author>() { }));
            _authorController = new AuthorController(_mockAuthorService.Object);

            var result = await _authorController.GetAllAuthorsAsync();
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            _mockAuthorService.Verify(x => x.GetAllAuthorsAsync(), Times.Once);
        }

        [TestMethod]
        public async Task Get_AllAuthors_OK()
        {
            List<Author> authors = new List<Author>() { new Author() };
            _mockAuthorService.Setup(x => x.GetAllAuthorsAsync()).Returns(Task.FromResult(authors));
            _authorController = new AuthorController(_mockAuthorService.Object);

            var result = await _authorController.GetAllAuthorsAsync();
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            _mockAuthorService.Verify(x => x.GetAllAuthorsAsync(), Times.Once);
        }

        [TestMethod]
        [DataRow("A")]
        [DataRow("B")]
        public async Task Create_Author_Ok(string name)
        {
            Author author = new Author() { Name = name };

            _mockAuthorService.Setup(x => x.CreateAuthorAsync(author)).Returns(Task.FromResult(1));
            _authorController = new AuthorController(_mockAuthorService.Object);

            var result = await _authorController.CreateAsync(author);
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            _mockAuthorService.Verify(x => x.CreateAuthorAsync(author), Times.Once);
        }
    }
}
