using AutoMapper;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OnlineLibrary.API.Controllers;
using OnlineLibrary.API.Model;
using OnlineLibrary.API.Validator;
using OnlineLibrary.BLL.Interfaces;
using OnlineLibrary.Common.DBEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineLibraryApiTest.Controllers
{
    [TestClass]
    public class UserControllerTest
    {
        private UserController _userController;

        private Mock<IUserService> _mockUserService = new Mock<IUserService>();

        private CreateUserValidator _createUserValidator = new CreateUserValidator();

        private Mock<ILogger<UserController>> _mockILogger = new Mock<ILogger<UserController>>();

        private Mock<IMapper> _mockMapper = new Mock<IMapper>();

        // Task<IActionResult> CreateAsync([FromBody] CreateUser cUser)

        [TestMethod]
        public async Task Create_User_Ok()
        {
            _mockMapper.Setup(x => x.Map<CreateUser, User>(It.IsAny<CreateUser>())).Returns(new User());
            _mockUserService.Setup(x => x.CreateUserAsync(It.IsAny<User>())).Returns(Task.FromResult(1));
            _userController = new UserController(_mockUserService.Object, _mockMapper.Object, _mockILogger.Object);

            var result = await _userController.CreateAsync(new CreateUser());
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            _mockMapper.Verify(x => x.Map<CreateUser, User>(It.IsAny<CreateUser>()), Times.Once);
            _mockUserService.Verify(x => x.CreateUserAsync(It.IsAny<User>()), Times.Once);
        }

        // Task<IActionResult> GetAllUsersAsync()
        
        [TestMethod]
        public async Task Get_AllUsers_Ok()
        { 
            _mockUserService.Setup(x => x.GetAllUsersAsync()).Returns(Task.FromResult(new List<User>()));
            _userController = new UserController(_mockUserService.Object, _mockMapper.Object, _mockILogger.Object);
            var result = await _userController.GetAllUsersAsync();
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        
            _mockUserService.Verify(x => x.GetAllUsersAsync(), Times.Once);
        }
        
        // Validate CreateUser - it passed as [FromBody] in CreateAsync method

        [TestMethod]
        [DataRow(null, "   ", -1, -1,-1)]
        [DataRow("", null, 1000, 13, 0)]
        [DataRow("  ", "email", 0, 0, 32)]
        public void Validate_CreateBook_FieldIsIncorrect(string name, string email, int year, int month, int day)
        {
            CreateUser user = new CreateUser() { Name = name, Email = email, Year = year, Month = month, Day = day };
            var result = _createUserValidator.TestValidate(user);
            result.ShouldHaveValidationErrorFor(x => x.Name);
            result.ShouldHaveValidationErrorFor(x => x.Email);
            result.ShouldHaveValidationErrorFor(x => x.Year);
            result.ShouldHaveValidationErrorFor(x => x.Month);
            result.ShouldHaveValidationErrorFor(x => x.Day);
        }
    }
}
