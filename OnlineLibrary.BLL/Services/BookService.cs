﻿using OnlineLibrary.Common.Entities;
using OnlineLibrary.Common.Helpers;
using OnlineLibrary.BLL.Interfaces;
using OnlineLibrary.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using OnlineLibrary.Common.Exceptions;
using OnlineLibrary.Common.Exceptions.Enum;
using Microsoft.AspNetCore.JsonPatch;
using OnlineLibrary.Common.Validators;

namespace OnlineLibrary.BLL.Services
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly BookValidator bookValidator;

        public BookService(IUnitOfWork uow)
        {
            unitOfWork = uow;
            bookValidator = new BookValidator();
        }

        public List<Book> GetAllBooks()
        {
            List<Book> books = unitOfWork.BookRepository.GetAllBooks();
            return books;
        }

        public List<Book> FilterBooks(int? authorId, string name, int? inReserve, int? inArchieve)
        {
            // Убираем отступы у строки.
            name = (name != null) ? name.Trim() : name;
            // authorId теперь либо null либо больше 0 (чтоб дальше долго не проверять).
            authorId = (authorId <= 0 || authorId == null) ? null : authorId;

            // Проверяем 0 случай, когда все поля фильтрации не заполнены.
            if (
                    (authorId == null)
                    && String.IsNullOrEmpty(name)
                    && (inReserve == null || inReserve < 0 || inReserve > 1)
                    && (inArchieve == null || inArchieve < 0 || inArchieve > 1)
                 )
            {
                List<Book> allBooks =  GetAllBooks();
                ExceptionHelper.Check<OLException>(allBooks == null || !allBooks.Any(), "Books don't exist", ExceptionType.NotFound);
                return allBooks;
            }

            List<Book> books;
            // Проверяем первый случай когда заполнены поля "имя" и "номер автора".
            if ((authorId != null) && !String.IsNullOrEmpty(name))
            {
                books = unitOfWork.BookRepository.FilterBooks((int)authorId, name);
            }
            // Заполнен только автор книги.
            else if (authorId != null)
            {
                books = unitOfWork.BookRepository.FilterBooks((int)authorId);
            }
            // Заполнено только название книги.
            else if (!String.IsNullOrEmpty(name))
            {
                books = unitOfWork.BookRepository.FilterBooks(name);
            }
            // Поля фильтрации пустые, получаем весь список.
            else
            {
                books = unitOfWork.BookRepository.GetAllBooks();
            }

            ExceptionHelper.Check<OLException>(books == null || !books.Any(), "There are no books matching this search", ExceptionType.NotFound);

            // Выбраны поля резервации и архивации.
            if (inReserve >= 0 && inReserve <= 1 && inArchieve >= 0 && inArchieve <= 1)
            {
                bool reservation = (inReserve == 0) ? false : true;
                bool archievation = (inArchieve == 0) ? false : true;
                books = books.Where(b => b.InArchive == archievation && b.Reserve == reservation).ToList();
            }
            // Выбраны поля резервации.
            else if (inReserve >= 0 && inReserve <= 1)
            {
                bool reservation = (inReserve == 0) ? false : true;
                books = books.Where(b => b.Reserve == reservation).ToList();
            }
            // Выбраны поля архивации.
            else if (inArchieve >= 0 && inArchieve <= 1)
            {
                bool archievation = (inArchieve == 0) ? false : true;
                books = books.Where(b => b.InArchive == archievation).ToList();
            }
            // Если проверка выше не прошла, значит поля резервации и архивации не выбраны.
            ExceptionHelper.Check<OLException>(books == null || !books.Any(), "There are no books matching this search", ExceptionType.NotFound);
            return books;
        }


        public Book GetBookById(int? bookId)
        {
            ExceptionHelper.Check<OLException>(bookId == null || bookId <= 0, "Id is incorrect", ExceptionType.BadRequest);
            Book book = unitOfWork.BookRepository.GetBookById((int)bookId);
            ExceptionHelper.Check<OLException>(book == null, "Book not found", ExceptionType.NotFound);
            return book;
        }

        public void ChangeBookReservation(int? bookId, bool newReservationValue)
        {
            ExceptionHelper.Check<OLException>(bookId == null || bookId <= 0, "Id is incorrect", ExceptionType.BadRequest);
            ExceptionHelper.Check<OLException>(!unitOfWork.BookRepository.IsBookIdExists((int)bookId), "Book not found", ExceptionType.NotFound);
            unitOfWork.BookRepository.ChangeBookReservation((int)bookId, newReservationValue);
            unitOfWork.Save();
        }

        public void ChangeBookArchievation(int? bookId, bool newArchievationValue)
        {
            ExceptionHelper.Check<OLException>(bookId == null || bookId <= 0, "Id is incorrect", ExceptionType.BadRequest);
            ExceptionHelper.Check<OLException>(!unitOfWork.BookRepository.IsBookIdExists((int)bookId), "Book not found", ExceptionType.NotFound);
            unitOfWork.BookRepository.ChangeBookArchievation((int)bookId, newArchievationValue);
            unitOfWork.Save();
        }
        public void UpdatePatch(int bookId, JsonPatchDocument<Book> book)
        {
            var originalBook = unitOfWork.BookRepository.GetBookById(bookId);
            ExceptionHelper.Check<OLException>(originalBook == null, "Book not found", ExceptionType.NotFound);
            book.ApplyTo(originalBook);
            var results = bookValidator.Validate(originalBook);
            ExceptionHelper.Check<OLException>(!results.IsValid, "The book has been changed incorrectly", ExceptionType.BadRequest);
            unitOfWork.Save();
        }

        public int CreateBook(Book book)
        {
            ExceptionHelper.Check<OLException>(book == null, "A null object came to the method", ExceptionType.BadRequest);
            unitOfWork.BookRepository.InsertBook(book);
            unitOfWork.Save();
            ExceptionHelper.Check<OLException>(book.Id == 0, "The book was not created", ExceptionType.InternalServerError);
            return book.Id;
        }
    }
}
