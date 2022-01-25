using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OnlineLibrary.API.Controllers;
using OnlineLibrary.BLL.Interfaces;
using OnlineLibrary.Common.Entities;
using OnlineLibrary.Common.Validators;
using System.Collections.Generic;

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
        public void Get_AllAuthors_ListIsEmpty_Ok()
        {
            _mockAuthorService.Setup(x => x.GetAllAuthors()).Returns(new List<Author>() { });
            _authorController = new AuthorController(_mockAuthorService.Object);

            var result = _authorController.GetAllAuthors();
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            _mockAuthorService.Verify(x => x.GetAllAuthors(), Times.Once);
        }

        [TestMethod]
        public void Get_AllAuthors_OK()
        {
            _mockAuthorService.Setup(x => x.GetAllAuthors()).Returns(new List<Author>() { new Author() });
            _authorController = new AuthorController(_mockAuthorService.Object);

            var result = _authorController.GetAllAuthors();
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            _mockAuthorService.Verify(x => x.GetAllAuthors(), Times.Once);
        }

        [TestMethod]
        [DataRow("A")]
        [DataRow("B")]
        public void Create_Author_Ok(string name)
        {
            Author author = new Author() { Name = name };

            _mockAuthorService.Setup(x => x.CreateAuthor(author)).Returns(1);
            _authorController = new AuthorController(_mockAuthorService.Object);

            var result = _authorController.Create(author);
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            _mockAuthorService.Verify(x => x.CreateAuthor(author), Times.Once);
        }
    }
}
