using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OnlineLibrary.API.Controllers;
using OnlineLibrary.BLL.Interfaces;
using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.Common.Validators;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineLibraryApiTest.Controllers
{
    [TestClass]
    public class UserControllerTest
    {
        private UserController _userController;

        private Mock<IUserService> _mockUserService = new Mock<IUserService>();

        private UserValidator _userValidator = new UserValidator();

        private Mock<ILogger<UserController>> _mockILogger = new Mock<ILogger<UserController>>();

        // Task<IActionResult> CreateAsync([FromBody] CreateUser cUser)

        [TestMethod]
        public async Task Create_User_Ok()
        {
            _mockUserService.Setup(x => x.CreateUserAsync(It.IsAny<User>())).Returns(Task.FromResult(1));
            _userController = new UserController(_mockUserService.Object, _mockILogger.Object);

            var result = await _userController.CreateAsync(new User());
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            _mockUserService.Verify(x => x.CreateUserAsync(It.IsAny<User>()), Times.Once);
        }

        // Task<IActionResult> GetAllUsersAsync()
        
        [TestMethod]
        public async Task Get_AllUsers_Ok()
        { 
            _mockUserService.Setup(x => x.GetAllUsersAsync()).Returns(Task.FromResult(new List<User>()));
            _userController = new UserController(_mockUserService.Object, _mockILogger.Object);
            var result = await _userController.GetAllUsersAsync();
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        
            _mockUserService.Verify(x => x.GetAllUsersAsync(), Times.Once);
        }
        
        // Validate CreateUser - it passed as [FromBody] in CreateAsync method

        [TestMethod]
        [DataRow(null, "   ", "1000-01-01")]
        [DataRow("", null, "3000-01-01")]
        [DataRow("  ", "email", "3000-01-01")]
        public void Validate_CreateBook_FieldIsIncorrect(string name, string email, string date)
        {
            User user = new User() { Name = name, Email = email, DateOfBirth = DateTime.Parse(date)};
            var result = _userValidator.TestValidate(user);
            result.ShouldHaveValidationErrorFor(x => x.Name);
            result.ShouldHaveValidationErrorFor(x => x.Email);
            result.ShouldHaveValidationErrorFor(x => x.DateOfBirth);
        }
    }
}
