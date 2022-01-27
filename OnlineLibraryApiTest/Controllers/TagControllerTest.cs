using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OnlineLibrary.API.Controllers;
using OnlineLibrary.BLL.Interfaces;
using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.Common.Validators;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineLibraryApiTest.Controllers
{
    [TestClass]
    public class TagControllerTest
    {
        private TagController _tagController;

        private Mock<ITagService> _mockTagService = new Mock<ITagService>();

        private TagValidator _tagValidator = new TagValidator();

        [TestMethod]
        [DataRow("")]
        [DataRow("  ")]
        [DataRow(null)]
        public void Validate_Tag_FieldIsIncorrect(string name)
        {
            Tag tag = new Tag() { Name = name };
            var result = _tagValidator.TestValidate(tag);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [TestMethod]
        public void Get_AllTags_ListIsEmpty_Ok()
        {
            _mockTagService.Setup(x => x.GetAllTags()).Returns(new List<Tag>() { });
            _tagController = new TagController(_mockTagService.Object);

            var result = _tagController.GetAllTags();
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            _mockTagService.Verify(x => x.GetAllTags(), Times.Once);
        }

        [TestMethod]
        public void Get_AllTags_OK()
        {
            _mockTagService.Setup(x => x.GetAllTags()).Returns(new List<Tag>() { new Tag() });
            _tagController = new TagController(_mockTagService.Object);

            var result = _tagController.GetAllTags();
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            _mockTagService.Verify(x => x.GetAllTags(), Times.Once);
        }

        [TestMethod]
        [DataRow("A")]
        [DataRow("B")]
        public void Create_Tag_Ok(string name)
        {
            Tag tag = new Tag() { Name = name };

            _mockTagService.Setup(x => x.CreateTag(tag)).Returns(1);
            _tagController = new TagController(_mockTagService.Object);

            var result = _tagController.Create(tag);
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            _mockTagService.Verify(x => x.CreateTag(tag), Times.Once);
        }
    }
}
