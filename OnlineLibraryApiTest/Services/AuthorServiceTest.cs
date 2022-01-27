using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OnlineLibrary.BLL.Services;
using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.Common.Exceptions;
using OnlineLibrary.Common.Exceptions.Enum;
using OnlineLibrary.DAL.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLibraryApiTest.Services
{
    [TestClass]
    public class AuthorServiceTest
    {
        private AuthorService _authorService;

        private Mock<IUnitOfWork> _mockUnitOfWork;

        private Mock<IBookRepository> _mockBookRepository;

        private Mock<IAuthorRepository> _mockAuthorRepository;

        [TestInitialize]
        public void InitializeTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockBookRepository = new Mock<IBookRepository>();
            _mockAuthorRepository = new Mock<IAuthorRepository>();
            _mockUnitOfWork.Setup(x => x.BookRepository).Returns(_mockBookRepository.Object);
            _mockUnitOfWork.Setup(x => x.AuthorRepository).Returns(_mockAuthorRepository.Object);
        }

        [TestMethod]
        public async Task Get_AllAuthors_ListIsEmpty()
        {
            _mockUnitOfWork.Setup(x => x.AuthorRepository.GetAllAuthorsAsync()).Returns(Task.FromResult(new List<Author>() { }));
            _authorService = new AuthorService(_mockUnitOfWork.Object);

            List<Author> authors = await _authorService.GetAllAuthorsAsync();
            Assert.IsFalse(authors.Any());
            _mockUnitOfWork.Verify(x => x.AuthorRepository.GetAllAuthorsAsync(), Times.Once);
        }

        [TestMethod]
        public async Task Get_AllAuthors_Ok()
        {
            List<Author> authors = new List<Author>() { new Author() };
            _mockUnitOfWork.Setup(x => x.AuthorRepository.GetAllAuthorsAsync()).Returns(Task.FromResult(authors));
            _authorService = new AuthorService(_mockUnitOfWork.Object);
            List<Author> result = await _authorService.GetAllAuthorsAsync();
            Assert.AreEqual(authors, result);
            _mockUnitOfWork.Verify(x => x.AuthorRepository.GetAllAuthorsAsync(), Times.Once);
        }

        [TestMethod]
        [DataRow(0,  -2 , 100)]
        [DataRow(1, -23 )]
        [DataRow( )]
        [ExpectedException(typeof(OLNotFound))]
        public async Task Get_AuthorsByIdList_ReturnsNothing(params int[] ids)
        {
            List<int> authorsId = ids.ToList();
            _mockUnitOfWork.Setup(x => x.AuthorRepository.GetAuthorsByIdListAsync(authorsId)).Returns(Task.FromResult(new List<Author>() { }));
            _authorService = new AuthorService(_mockUnitOfWork.Object);

            await _authorService.GetAuthorsByIdListAsync(authorsId);
            _mockUnitOfWork.Verify(x => x.AuthorRepository.GetAuthorsByIdListAsync(authorsId), Times.Once);
        }

        [TestMethod]
        [DataRow(0, -2, 100)]
        [DataRow(1, -23)]
        [DataRow(1, 2, 3)]
        public async Task Get_AuthorsByIdList_OK(params int[] ids)
        {
            List<int> authorsId = ids.ToList();
            List<Author> authors = new List<Author>() { new Author() };

            _mockUnitOfWork.Setup(x => x.AuthorRepository.GetAuthorsByIdListAsync(authorsId)).Returns(Task.FromResult(authors));
            _authorService = new AuthorService(_mockUnitOfWork.Object);

            List<Author> result = await _authorService.GetAuthorsByIdListAsync(authorsId);

            Assert.AreEqual(authors, result);
            _mockUnitOfWork.Verify(x => x.AuthorRepository.GetAuthorsByIdListAsync(authorsId), Times.Once);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        [DataRow(null)]
        [ExpectedException(typeof(OLBadRequest))]
        public async Task Create_Author_FieldsIsIncorrect(string name)
        {
            _mockAuthorRepository.Setup(x => x.InsertAuthor(It.IsAny<Author>()));
            _authorService = new AuthorService(_mockUnitOfWork.Object);
            await _authorService.CreateAuthorAsync(new Author() { Name = name });
            _mockUnitOfWork.Verify(x => x.AuthorRepository.InsertAuthor(It.IsAny<Author>()), Times.Once);
        }

        [TestMethod]
        [DataRow("s")]
        public async Task Create_Author_OK(string name)
        {
            Author author = new Author() { Id = 1, Name = name };
            _mockUnitOfWork.Setup(x => x.AuthorRepository.InsertAuthor(It.IsAny<Author>()));
            _authorService = new AuthorService(_mockUnitOfWork.Object);
            int? id = await _authorService.CreateAuthorAsync(author);

            _mockUnitOfWork.Verify(x => x.AuthorRepository.InsertAuthor(It.IsAny<Author>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveAsync(), Times.Once);
        }
    }
}
