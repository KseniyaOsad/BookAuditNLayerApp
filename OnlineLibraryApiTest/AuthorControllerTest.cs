using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OnlineLibrary.API.Controllers;
using OnlineLibrary.BLL.Interfaces;
using OnlineLibrary.Common.Entities;
using System.Collections.Generic;

namespace OnlineLibraryApiTest
{
    [TestClass]
    public class AuthorControllerTest
    {
        private AuthorController authorController;

        private Mock<IAuthorService> mockAuthorService = new Mock<IAuthorService>();

        [TestMethod]
        [DataRow("")]
        [DataRow("  ")]
        [DataRow(null)]
        public void Create_Author_FieldIsIncorrect(string name)
        {
            Author author = new Author() { Name = name };
            mockAuthorService.Setup(x => x.CreateAuthor(author)).Returns(0);
            authorController = new AuthorController(mockAuthorService.Object);
            var result = authorController.Create(author);
            mockAuthorService.Verify(x => x.CreateAuthor(author), Times.Once);

        }

        [TestMethod]
        public void Get_AllAuthors_ListIsEmpty_Ok()
        {
            mockAuthorService.Setup(x => x.GetAllAuthors()).Returns(new List<Author>() { });
            authorController = new AuthorController(mockAuthorService.Object);

            var result = authorController.GetAllAuthors();
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            mockAuthorService.Verify(x => x.GetAllAuthors(), Times.Once);
        }

        [TestMethod]
        public void Get_AllAuthors_OK()
        {
            mockAuthorService.Setup(x => x.GetAllAuthors()).Returns(new List<Author>() { new Author() });
            authorController = new AuthorController(mockAuthorService.Object);

            var result = authorController.GetAllAuthors();
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            mockAuthorService.Verify(x => x.GetAllAuthors(), Times.Once);
        }

        [TestMethod]
        [DataRow("A")]
        [DataRow("B")]
        public void Create_Author_Ok(string name)
        {
            Author author = new Author() { Name = name };

            mockAuthorService.Setup(x => x.CreateAuthor(author)).Returns(1);
            authorController = new AuthorController(mockAuthorService.Object);

            var result = authorController.Create(author);
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            mockAuthorService.Verify(x => x.CreateAuthor(author), Times.Once);
        }
    }
}
