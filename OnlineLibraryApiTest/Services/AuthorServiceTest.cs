using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OnlineLibrary.BLL.Services;
using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.Common.Exceptions;
using OnlineLibrary.DAL.Interfaces;
using System.Collections.Generic;
using System.Linq;

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
        public void Get_AllAuthors_ListIsEmpty()
        {
            _mockUnitOfWork.Setup(x => x.AuthorRepository.GetAllAuthors()).Returns(new List<Author>() { });
            _authorService = new AuthorService(_mockUnitOfWork.Object);

            List<Author> authors = _authorService.GetAllAuthors();
            Assert.IsFalse(authors.Any());
            _mockUnitOfWork.Verify(x => x.AuthorRepository.GetAllAuthors(), Times.Once);
        }

        [TestMethod]
        public void Get_AllAuthors_Ok()
        {
            List<Author> authors = new List<Author>() { new Author() };
            _mockUnitOfWork.Setup(x => x.AuthorRepository.GetAllAuthors()).Returns(authors);
            _authorService = new AuthorService(_mockUnitOfWork.Object);
            List<Author> result = _authorService.GetAllAuthors();
            Assert.AreEqual(authors, result);
            _mockUnitOfWork.Verify(x => x.AuthorRepository.GetAllAuthors(), Times.Once);
        }

        [TestMethod]
        [DataRow(0,  -2 , 100)]
        [DataRow(1, -23 )]
        [DataRow( )]
        public void Get_AuthorsByIdList_ReturnsNothing(params int[] ids)
        {
            List<int> authorsId = ids.ToList();
            _mockUnitOfWork.Setup(x => x.AuthorRepository.GetAuthorsByIdList(authorsId)).Returns(new List<Author>() { });
            _authorService = new AuthorService(_mockUnitOfWork.Object);

            Assert.ThrowsException<OLNotFound>(() => _authorService.GetAuthorsByIdList(authorsId), "Expected Exception");
            _mockUnitOfWork.Verify(x => x.AuthorRepository.GetAuthorsByIdList(authorsId), Times.Once);
        }

        [TestMethod]
        [DataRow(0, -2, 100)]
        [DataRow(1, -23)]
        [DataRow(1, 2, 3)]
        public void Get_AuthorsByIdList_OK(params int[] ids)
        {
            List<int> authorsId = ids.ToList();
            List<Author> authors = new List<Author>() { new Author() };

            _mockUnitOfWork.Setup(x => x.AuthorRepository.GetAuthorsByIdList(authorsId)).Returns(authors);
            _authorService = new AuthorService(_mockUnitOfWork.Object);

            List<Author> result = _authorService.GetAuthorsByIdList(authorsId);

            Assert.AreEqual(authors, result);
            _mockUnitOfWork.Verify(x => x.AuthorRepository.GetAuthorsByIdList(authorsId), Times.Once);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        [DataRow(null)]
        public void Create_Author_FieldsIsIncorrect(string name)
        {
            _mockAuthorRepository.Setup(x => x.InsertAuthor(It.IsAny<Author>()));
            _authorService = new AuthorService(_mockUnitOfWork.Object);
            Assert.ThrowsException<OLBadRequest>(() => _authorService.CreateAuthor(new Author() { Name = name }), "Expected Exception");
            _mockUnitOfWork.Verify(x => x.AuthorRepository.InsertAuthor(It.IsAny<Author>()), Times.Once);
        }

        [TestMethod]
        [DataRow("s")]
        public void Create_Author_OK(string name)
        {
            Author author = new Author() { Id = 1, Name = name };
            _mockUnitOfWork.Setup(x => x.AuthorRepository.InsertAuthor(It.IsAny<Author>()));
            _authorService = new AuthorService(_mockUnitOfWork.Object);
            int? id = _authorService.CreateAuthor(author);

            _mockUnitOfWork.Verify(x => x.AuthorRepository.InsertAuthor(It.IsAny<Author>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);
        }
    }
}
