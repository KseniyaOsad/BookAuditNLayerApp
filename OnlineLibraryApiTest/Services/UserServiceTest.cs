using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OnlineLibrary.BLL.Services;
using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.Common.Exceptions;
using OnlineLibrary.DAL.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineLibraryApiTest.Services
{
    [TestClass]
    public class UserServiceTest
    {
        private UserService _userService;

        private Mock<IUnitOfWork> _mockUnitOfWork = new Mock<IUnitOfWork>();

        private Mock<IUserRepository> _mockUserRepository = new Mock<IUserRepository>();

        [TestInitialize]
        public void InitializeTest()
        {
            _mockUnitOfWork.Setup(x => x.UserRepository).Returns(_mockUserRepository.Object);
        }

        // Task<int> CreateUserAsync(User user)

        [TestMethod]
        public async Task Create_User_BadRequest()
        {
            _mockUserRepository.Setup(x => x.CreateUserAsync(It.IsAny<User>()));
            _userService = new UserService(_mockUnitOfWork.Object);
            await Assert.ThrowsExceptionAsync<OLBadRequest>(() => _userService.CreateUserAsync(new User()));
            _mockUnitOfWork.Verify(x => x.UserRepository.CreateUserAsync(It.IsAny<User>()), Times.Once);
        }

        [TestMethod]
        public async Task Create_User_Ok()
        {
            _mockUserRepository.Setup(x => x.CreateUserAsync(It.IsAny<User>()));
            _userService = new UserService(_mockUnitOfWork.Object);
            int id = await _userService.CreateUserAsync(new User() { Id = 1});
            Assert.AreEqual(1, id);
            _mockUnitOfWork.Verify(x => x.UserRepository.CreateUserAsync(It.IsAny<User>()), Times.Once);
        }

        // Task<List<User>> GetAllUsersAsync()

        [TestMethod]
        public async Task Get_AllUsers_Ok()
        {
            _mockUnitOfWork.Setup(x => x.UserRepository.GetAllUsersAsync()).Returns(Task.FromResult(new List<User>()));
            _userService = new UserService(_mockUnitOfWork.Object);
            await _userService.GetAllUsersAsync();
            _mockUnitOfWork.Verify(x => x.UserRepository.GetAllUsersAsync(), Times.Once);
        }
    }
}
