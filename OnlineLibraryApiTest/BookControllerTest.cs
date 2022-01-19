﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OnlineLibrary.API.Controllers;
using OnlineLibrary.API.Model;
using OnlineLibrary.BLL.Interfaces;
using OnlineLibrary.Common.Entities;
using System.Collections.Generic;
using System.Linq;

namespace OnlineLibraryApiTest
{
    [TestClass]
    public class BookControllerTest
    {
        private BookController bookController;

        private Mock<IBookService> mockBookService = new Mock<IBookService>();

        private Mock<IAuthorService> mockAuthorService = new Mock<IAuthorService>();

        [TestMethod]
        [DataRow("", "s", 3, new int[] { 1, 2 })]
        [DataRow("s", "", 2, new int[] { 4 })]
        [DataRow(null, "s", 1, new int[] { })]
        [DataRow("s", "s", 1, new int[] { })]
        public void Create_Book(string name, string descr, int? genre, int[] authors)
        {
            List<int> ints = authors.ToList();
            CreateBook cBook = new CreateBook() { Name = name, Description = descr, Genre = genre, Authors = ints };

            mockAuthorService.Setup(x => x.GetAuthorsByIdList(ints));
            mockBookService.Setup(x => x.CreateBook(It.IsAny<Book>()));
            bookController = new BookController(mockBookService.Object, mockAuthorService.Object);

            bookController.Create(cBook);
            mockAuthorService.Verify(x => x.GetAuthorsByIdList(ints), Times.Once);
            mockBookService.Verify(x => x.CreateBook(It.IsAny<Book>()), Times.Once);
        }

        [TestMethod]
        [DataRow("s", "s", null, new int[] { 1, 2 })]
        [DataRow("s", "s", 100, new int[] { 1, 2 })]
        [DataRow("s", "s", -1, new int[] { 1, 2 })]
        public void Create_Book_GenreIsIncorrect(string name, string descr, int? genre, int[] authors)
        {
            List<int> ints = authors.ToList();
            CreateBook cBook = new CreateBook() { Name = name, Description = descr, Genre = genre, Authors = ints };

            mockAuthorService.Setup(x => x.GetAuthorsByIdList(ints));
            mockBookService.Setup(x => x.CreateBook(It.IsAny<Book>()));
            bookController = new BookController(mockBookService.Object, mockAuthorService.Object);

            bookController.Create(cBook);
            mockAuthorService.Verify(x => x.GetAuthorsByIdList(ints), Times.Never);
            mockBookService.Verify(x => x.CreateBook(It.IsAny<Book>()), Times.Never);
        }

        [TestMethod]
        [DataRow(-1, true)]
        [DataRow(1, false)]
        [DataRow(0, false)]
        public void Change_BookArchivation(int id, bool arch)
        {
            Book book = new Book() { Id = id, InArchive = arch };
            mockBookService.Setup(x => x.ChangeBookArchievation(id, arch));
            mockBookService.Setup(x => x.GetBookById(id));
            bookController = new BookController(mockBookService.Object, mockAuthorService.Object);

            bookController.UpdateArchievation(id, book);
            mockBookService.Verify(x => x.ChangeBookArchievation(id, arch), Times.Once);
            mockBookService.Verify(x => x.GetBookById(id), Times.Once);
        }

        [TestMethod]
        [DataRow(-1, true)]
        [DataRow(1, false)]
        [DataRow(0, false)]
        public void Change_BookReservation(int id, bool reserve)
        {
            Book book = new Book() { Id = id, Reserve = reserve };
            mockBookService.Setup(x => x.ChangeBookArchievation(id, reserve));
            mockBookService.Setup(x => x.GetBookById(id));
            bookController = new BookController(mockBookService.Object, mockAuthorService.Object);

            bookController.UpdateReservation(id, book);
            mockBookService.Verify(x => x.ChangeBookReservation(id, reserve), Times.Once);
            mockBookService.Verify(x => x.GetBookById(id), Times.Once);
        }

        [TestMethod]
        public void Get_AllBooks_OK()
        {
            mockBookService.Setup(x => x.GetAllBooks()).Returns(new List<Book>() {  });
            bookController = new BookController(mockBookService.Object, mockAuthorService.Object);

            var result = bookController.GetAllBooks();
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            mockBookService.Verify(x => x.GetAllBooks(), Times.Once);
        }

        [TestMethod]
        [DataRow(1, "C#", 0, 1)]
        [DataRow(null, "C#", 1, 0)]
        [DataRow(null, "", null, null)]
        [DataRow(null, "C#", null, 0)]
        [DataRow(null, "C#", 1, null)]
        [DataRow(null, null, null, null)]
        [DataRow(1, null, 1, null)]
        public void Get_BooksWithFilters_Ok(int? authorId, string name, int? res, int? arch)
        {
            mockBookService.Setup(x => x.FilterBooks(authorId, name, res, arch)).Returns(new List<Book>() { new Book() });
            bookController = new BookController(mockBookService.Object, mockAuthorService.Object);

            var result = bookController.GetBooksWithFilters(authorId, name, res, arch);
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            mockBookService.Verify(x => x.FilterBooks(authorId, name, res, arch), Times.Once);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(null)]
        [DataRow(-3)]
        public void Get_BookById_Ok(int? bookId)
        {
            mockBookService.Setup(x => x.GetBookById(bookId)).Returns(new Book());
            bookController = new BookController(mockBookService.Object, mockAuthorService.Object);

            var result = bookController.GetBookById(bookId);
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            mockBookService.Verify(x => x.GetBookById(bookId), Times.Once);

        }

        [TestMethod]
        [DataRow("s", "s", 1, new int[] { 1, 2 })]
        [DataRow("a", "a", 2, new int[] { 1 })]
        public void Create_Book_Ok(string name, string descr, int? genre, int[] authors)
        {
            CreateBook cBook = new CreateBook() { Name = name, Description = descr, Genre = genre, Authors = authors.ToList() };

            mockAuthorService.Setup(x => x.GetAuthorsByIdList(It.IsAny<List<int>>())).Returns(new List<Author>() { new Author() });
            mockBookService.Setup(x => x.CreateBook(It.IsAny<Book>())).Returns(1);
            bookController = new BookController(mockBookService.Object, mockAuthorService.Object);

            var result = bookController.Create(cBook);
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            mockAuthorService.Verify(x => x.GetAuthorsByIdList(It.IsAny<List<int>>()), Times.Once);
            mockBookService.Verify(x => x.CreateBook(It.IsAny<Book>()), Times.Once);

        }

        [TestMethod]
        [DataRow(1, true)]
        [DataRow(2, false)]
        [DataRow(3, false)]
        public void Change_BookArchivation_Ok(int id, bool arch)
        {
            Book book = new Book() { Id = id, InArchive = arch };

            mockBookService.Setup(x => x.GetBookById(id)).Returns(new Book());
            bookController = new BookController(mockBookService.Object, mockAuthorService.Object);

            var result = bookController.UpdateArchievation(id, book);
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            mockBookService.Verify(x => x.ChangeBookArchievation(id, arch), Times.Once);
            mockBookService.Verify(x => x.GetBookById(id), Times.Once);
        }

        [TestMethod]
        [DataRow(1, true)]
        [DataRow(2, false)]
        [DataRow(3, false)]
        public void Change_BookReservation_Ok(int id, bool reserve)
        {
            Book book = new Book() { Id = id, Reserve = reserve };

            mockBookService.Setup(x => x.GetBookById(id)).Returns(new Book());
            bookController = new BookController(mockBookService.Object, mockAuthorService.Object);

            var result = bookController.UpdateReservation(id, book);
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            mockBookService.Verify(x => x.ChangeBookReservation(id, reserve), Times.Once);
            mockBookService.Verify(x => x.GetBookById(id), Times.Once);
        }
    }
}
