﻿using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
    public class TagControllerTest
    {
        private TagController _tagController;

        private Mock<ITagService> _mockTagService = new Mock<ITagService>();

        private TagValidator _tagValidator = new TagValidator();

        private Mock<ILogger<TagController>> _mockILogger = new Mock<ILogger<TagController>>();

        // Task<IActionResult> CreateAsync([FromBody] Tag tag)

        [TestMethod]
        [DataRow("A")]
        [DataRow("B")]
        public async Task Create_Tag_Ok(string name)
        {
            Tag tag = new Tag() { Name = name };

            _mockTagService.Setup(x => x.CreateTagAsync(tag)).Returns(Task.FromResult(1));
            _tagController = new TagController(_mockTagService.Object, _mockILogger.Object);

            var result = await _tagController.CreateAsync(tag);
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            _mockTagService.Verify(x => x.CreateTagAsync(tag), Times.Once);
        }

        // Task<IActionResult> GetAllTagsAsync()

        [TestMethod]
        public async Task Get_AllTags_ListIsEmpty_Ok()
        {
            _mockTagService.Setup(x => x.GetAllTagsAsync()).Returns(Task.FromResult(new List<Tag>() { }));
            _tagController = new TagController(_mockTagService.Object, _mockILogger.Object);

            var result = await _tagController.GetAllTagsAsync();
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            _mockTagService.Verify(x => x.GetAllTagsAsync(), Times.Once);
        }

        [TestMethod]
        public async Task Get_AllTags_OK()
        {
            _mockTagService.Setup(x => x.GetAllTagsAsync()).Returns(Task.FromResult(new List<Tag>() { new Tag() }));
            _tagController = new TagController(_mockTagService.Object, _mockILogger.Object);

            var result = await _tagController.GetAllTagsAsync();
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            _mockTagService.Verify(x => x.GetAllTagsAsync(), Times.Once);
        }

        // Validate Tag - it passed as [FromBody] in CreateAsync method

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

    }
}
