using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OnlineLibrary.BLL.Services;
using OnlineLibrary.Common.Entities;
using OnlineLibrary.Common.Exceptions;
using OnlineLibrary.DAL.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace OnlineLibraryApiTest
{
    [TestClass]
    public class AuthorServiceTest
    {
        private AuthorService authorService;

        private Mock<IUnitOfWork> mockUnitOfWork;

        private Mock<IBookRepository> mockBookRepository;

        private Mock<IAuthorRepository> mockAuthorRepository;

        [TestInitialize]
        public void InitializeTest()
        {
            mockUnitOfWork = new Mock<IUnitOfWork>();
            mockBookRepository = new Mock<IBookRepository>();
            mockAuthorRepository = new Mock<IAuthorRepository>();
            mockUnitOfWork.Setup(x => x.BookRepository).Returns(mockBookRepository.Object);
            mockUnitOfWork.Setup(x => x.AuthorRepository).Returns(mockAuthorRepository.Object);
        }

        [TestMethod]
        public void Get_AllAuthors_ListIsEmpty()
        {
            mockUnitOfWork.Setup(x => x.AuthorRepository.GetAllAuthors()).Returns(new List<Author>() { });
            authorService = new AuthorService(mockUnitOfWork.Object);

            List<Author> authors = authorService.GetAllAuthors();
            Assert.IsFalse(authors.Any());
            mockUnitOfWork.Verify(x => x.AuthorRepository.GetAllAuthors(), Times.Once);
        }

        [TestMethod]
        public void Get_AllAuthors_Ok()
        {
            List<Author> authors = new List<Author>() { new Author() };
            mockUnitOfWork.Setup(x => x.AuthorRepository.GetAllAuthors()).Returns(authors);
            authorService = new AuthorService(mockUnitOfWork.Object);
            List<Author> result = authorService.GetAllAuthors();
            Assert.AreEqual(authors, result);
            mockUnitOfWork.Verify(x => x.AuthorRepository.GetAllAuthors(), Times.Once);
        }

        [TestMethod]
        [DataRow(0,  -2 , 100)]
        [DataRow(1, -23 )]
        [DataRow( )]
        public void Get_AuthorsByIdList_ReturnsNothing(params int[] ids)
        {
            List<int> authorsId = ids.ToList();
            mockUnitOfWork.Setup(x => x.AuthorRepository.GetAuthorsByIdList(authorsId)).Returns(new List<Author>() { });
            authorService = new AuthorService(mockUnitOfWork.Object);

            Assert.ThrowsException<OLNotFound>(() => authorService.GetAuthorsByIdList(authorsId), "Expected Exception");
            mockUnitOfWork.Verify(x => x.AuthorRepository.GetAuthorsByIdList(authorsId), Times.Once);
        }

        [TestMethod]
        [DataRow(0, -2, 100)]
        [DataRow(1, -23)]
        [DataRow(1, 2, 3)]
        public void Get_AuthorsByIdList_OK(params int[] ids)
        {
            List<int> authorsId = ids.ToList();
            List<Author> authors = new List<Author>() { new Author() };

            mockUnitOfWork.Setup(x => x.AuthorRepository.GetAuthorsByIdList(authorsId)).Returns(authors);
            authorService = new AuthorService(mockUnitOfWork.Object);

            List<Author> result = authorService.GetAuthorsByIdList(authorsId);

            Assert.AreEqual(authors, result);
            mockUnitOfWork.Verify(x => x.AuthorRepository.GetAuthorsByIdList(authorsId), Times.Once);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        [DataRow(null)]
        public void Create_Author_FieldsIsIncorrect(string name)
        {
            mockAuthorRepository.Setup(x => x.InsertAuthor(It.IsAny<Author>()));
            authorService = new AuthorService(mockUnitOfWork.Object);
            Assert.ThrowsException<OLBadRequest>(() => authorService.CreateAuthor(new Author() { Name = name }), "Expected Exception");
            mockUnitOfWork.Verify(x => x.AuthorRepository.InsertAuthor(It.IsAny<Author>()), Times.Once);
        }

        [TestMethod]
        [DataRow("s")]
        public void Create_Author_OK(string name)
        {
            Author author = new Author() { Id = 1, Name = name };
            mockUnitOfWork.Setup(x => x.AuthorRepository.InsertAuthor(It.IsAny<Author>()));
            authorService = new AuthorService(mockUnitOfWork.Object);
            int? id = authorService.CreateAuthor(author);

            mockUnitOfWork.Verify(x => x.AuthorRepository.InsertAuthor(It.IsAny<Author>()), Times.Once);
            mockUnitOfWork.Verify(x => x.Save(), Times.Once);
        }
    }
}
