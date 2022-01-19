using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OnlineLibrary.BLL.Services;
using OnlineLibrary.Common.Entities;
using OnlineLibrary.DAL.Interfaces;
using OnlineLibraryApiTest.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnlineLibraryApiTest
{
    [TestClass]
    public class AuthorServiceTest
    {
        private AuthorService authorService;
        private Mock<IUnitOfWork> mockUnitOfWork;

        [TestInitialize]
        public void InitializeTest()
        {
            mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(x => x.Book).Returns(new TestBookRepository());
            mockUnitOfWork.Setup(x => x.Author).Returns(new TestAuthorRepository());
        }


        [TestMethod]
        public void Get_AllAuthors_ListIsEmpty()
        {
            mockUnitOfWork.Setup(x => x.Author.GetAllAuthors()).Returns(new List<Author>() { });
            authorService = new AuthorService(mockUnitOfWork.Object);

            Assert.ThrowsException<Exception>(() => authorService.GetAllAuthors(), "Expected Exception");
            mockUnitOfWork.Verify(x => x.Author.GetAllAuthors(), Times.Once);
        }

        [TestMethod]
        public void Get_AllAuthors_Ok()
        {
            List<Author> authors = new List<Author>() { new Author() };
            mockUnitOfWork.Setup(x => x.Author.GetAllAuthors()).Returns(authors);
            authorService = new AuthorService(mockUnitOfWork.Object);
            List<Author> result = authorService.GetAllAuthors();
            Assert.AreEqual(authors, result);
            mockUnitOfWork.Verify(x => x.Author.GetAllAuthors(), Times.Once);
        }

        [TestMethod]
        [DataRow(0,  -2 , 100)]
        [DataRow(1, -23 )]
        [DataRow( )]
        public void Get_AuthorsByIdList_ReturnsNothing(params int[] ids)
        {
            List<int> authorsId = ids.ToList();
            mockUnitOfWork.Setup(x => x.Author.GetAuthorsByIdList(authorsId)).Returns(new List<Author>() { });
            authorService = new AuthorService(mockUnitOfWork.Object);

            Assert.ThrowsException<Exception>(() => authorService.GetAuthorsByIdList(authorsId), "Expected Exception");
            mockUnitOfWork.Verify(x => x.Author.GetAuthorsByIdList(authorsId), Times.Once);
        }


        [TestMethod]
        [DataRow(0, -2, 100)]
        [DataRow(1, -23)]
        [DataRow(1, 2, 3)]
        public void Get_AuthorsByIdList_OK(params int[] ids)
        {
            List<int> authorsId = ids.ToList();
            List<Author> authors = new List<Author>() { new Author() };

            mockUnitOfWork.Setup(x => x.Author.GetAuthorsByIdList(authorsId)).Returns(authors);
            authorService = new AuthorService(mockUnitOfWork.Object);

            List<Author> result = authorService.GetAuthorsByIdList(authorsId);

            Assert.AreEqual(authors, result);
            mockUnitOfWork.Verify(x => x.Author.GetAuthorsByIdList(authorsId), Times.Once);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        [DataRow(null)]
        public void Create_Author_FieldsIsIncorrect(string name)
        {
            authorService = new AuthorService(mockUnitOfWork.Object);
            Assert.ThrowsException<Exception>(() => authorService.CreateAuthor(new Author() { Name = name }), "Expected Exception");
            mockUnitOfWork.Verify(x => x.Author.CreateAuthor(It.IsAny<Author>()), Times.Never);
        }

        [TestMethod]
        [DataRow("s")]
        public void Create_Author_OK(string name)
        {
            Author author = new Author() { Name = name };
            authorService = new AuthorService(mockUnitOfWork.Object);
            int? id = authorService.CreateAuthor(author);

            mockUnitOfWork.Verify(x => x.Author.CreateAuthor(It.IsAny<Author>()), Times.Once);
            mockUnitOfWork.Verify(x => x.Save(), Times.Once);
        }

    }
}
