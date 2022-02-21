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
    public class AuthorServiceTest
    {
        private AuthorService _authorService;

        private Mock<IUnitOfWork> _mockUnitOfWork = new Mock<IUnitOfWork>();

        private Mock<IAuthorRepository> _mockAuthorRepository = new Mock<IAuthorRepository>();

        [TestInitialize]
        public void InitializeTest()
        {
            _mockUnitOfWork.Setup(x => x.AuthorRepository).Returns(_mockAuthorRepository.Object);
        }

        // Task<int> CreateAuthorAsync(Author author)

        [TestMethod]
        public async Task Create_Author_BadRequest()
        {
            _mockAuthorRepository.Setup(x => x.CreateAuthorAsync(It.IsAny<Author>()));
            _authorService = new AuthorService(_mockUnitOfWork.Object);
            await Assert.ThrowsExceptionAsync<OLBadRequest>(() => _authorService.CreateAuthorAsync(new Author()));
            _mockUnitOfWork.Verify(x => x.AuthorRepository.CreateAuthorAsync(It.IsAny<Author>()), Times.Once);
        }

        [TestMethod]
        public async Task Create_Author_Ok()
        {
            _mockAuthorRepository.Setup(x => x.CreateAuthorAsync(It.IsAny<Author>()));
            _authorService = new AuthorService(_mockUnitOfWork.Object);
            int id = await _authorService.CreateAuthorAsync(new Author() { Id = 1});
            Assert.AreEqual(1, id);
            _mockUnitOfWork.Verify(x => x.AuthorRepository.CreateAuthorAsync(It.IsAny<Author>()), Times.Once);
        }

        // Task<List<Author>> GetAllAuthorsAsync()

        [TestMethod]
        public async Task Get_AllAuthors_Ok()
        {
            _mockUnitOfWork.Setup(x => x.AuthorRepository.GetAllAuthorsAsync()).Returns(Task.FromResult(new List<Author>()));
            _authorService = new AuthorService(_mockUnitOfWork.Object);
            await _authorService.GetAllAuthorsAsync();
            _mockUnitOfWork.Verify(x => x.AuthorRepository.GetAllAuthorsAsync(), Times.Once);
        }

        // Task<List<Author>> GetAuthorsByIdListAsync(List<int> authorsId)

        [TestMethod]
        public async Task Get_AuthorsByIdList_ReturnsNothing()
        {
            _mockUnitOfWork.Setup(x => x.AuthorRepository.GetAuthorsByIdListAsync(It.IsAny<List<int>>())).Returns(Task.FromResult(new List<Author>()));
            _authorService = new AuthorService(_mockUnitOfWork.Object);

            await Assert.ThrowsExceptionAsync<OLNotFound>(() => _authorService.GetAuthorsByIdListAsync(new List<int>()));
            _mockUnitOfWork.Verify(x => x.AuthorRepository.GetAuthorsByIdListAsync(It.IsAny<List<int>>()), Times.Once);
        }

        [TestMethod]
        public async Task Get_AuthorsByIdList_OK()
        {
            _mockUnitOfWork.Setup(x => x.AuthorRepository.GetAuthorsByIdListAsync(It.IsAny<List<int>>())).Returns(Task.FromResult(new List<Author>() { new Author()}));
            _authorService = new AuthorService(_mockUnitOfWork.Object);

            List<Author> result = await _authorService.GetAuthorsByIdListAsync(new List<int>());

            _mockUnitOfWork.Verify(x => x.AuthorRepository.GetAuthorsByIdListAsync(It.IsAny<List<int>>()), Times.Once);
        }
    }
}
