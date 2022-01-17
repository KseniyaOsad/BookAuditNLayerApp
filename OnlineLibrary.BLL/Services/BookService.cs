﻿using OnlineLibrary.Common.Entities;
using OnlineLibrary.Common.Enums;
using OnlineLibrary.BLL.Infrastructure;
using OnlineLibrary.BLL.Interfaces;
using OnlineLibrary.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnlineLibrary.BLL.Services
{
    public class BookService : IBookService<Book>
    {
        IUnitOfWork Database { get; set; }
        public BookService(IUnitOfWork uow)
        {
            Database = uow;
        }

        public List<Book> GetAllBooks()
        {
            List<Book> books = Database.Book.GetAllBooks();
            if (books.Count == 0 || books == null)
            {
                throw new ValidationException("Книг нет", ErrorList.ListIsEmpty);
            }
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
                return GetAllBooks();
            }

            List<Book> books;
            // Проверяем первый случай когда заполнены поля "имя" и "номер автора".
            if ((authorId != null) && !String.IsNullOrEmpty(name))
            {
                books = Database.Book.FilterBooks((int)authorId, name);
            }
            // Заполнен только автор книги.
            else if (authorId != null)
            {
                books = Database.Book.FilterBooks((int)authorId);
            }
            // Заполнено только название книги.
            else if (!String.IsNullOrEmpty(name))
            {
                books = Database.Book.FilterBooks(name);
            }
            // Поля фильтрации пустые, получаем весь список.
            else
            {
                books = Database.Book.GetAllBooks();
            }

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
            if (books.Count == 0 || books == null)
            {
                throw new ValidationException("Книг по данному запросу нет", ErrorList.SearchReturnedNothing);
            }
            return books;
        }


        public Book GetBookById(int? bookId)
        {
            if (bookId == null || bookId <= 0)
            {
                throw new ValidationException("Id указан неправильно", ErrorList.IncorrectId );
            }
            try
            {
                Book book = Database.Book.GetBookById((int)bookId);
                if (book == null)
                {
                    throw new ValidationException("Книга не найдена", ErrorList.NotFound);
                }
                return book;
            }
            catch (Exception e)
            {
                throw new ValidationException(e.Message, ErrorList.NotFound);
            }
        }

        public void ChangeBookReservation(int? bookId, bool newReservationValue)
        {
            if (bookId == null || bookId < 1)
            {
                throw new ValidationException("Id указан неправильно", ErrorList.IncorrectId );
            }
            else if (!Database.Book.IsBookIdExists((int)bookId))
            {
                throw new ValidationException("Книга не найдена", ErrorList.NotFound);
            }
            Database.Book.ChangeBookReservation((int)bookId, newReservationValue);
            Database.Save();
        }

        public void ChangeBookArchievation(int? bookId, bool newArchievationValue)
        {
            if (bookId == null || bookId < 1)
            {
                throw new ValidationException("Id указан неправильно", ErrorList.IncorrectId);
            }
            else if (!Database.Book.IsBookIdExists((int)bookId))
            {
                throw new ValidationException("Книга не найдена", ErrorList.NotFound);
            }
            Database.Book.ChangeBookArchievation((int)bookId, newArchievationValue);
            Database.Save();
        }

        public int CreateBook(Book book)
        {
            if (book.Name == null || book.Name.Trim() == "")
            {
                throw new ValidationException("Поле 'Имя' заполнено неверно", ErrorList.FieldIsIncorrect);
            }
            else if (book.Description == null || book.Description.Trim() == "")
            {
                throw new ValidationException("Поле 'Описание' заполнено неверно", ErrorList.FieldIsIncorrect);
            }
            Database.Book.CreateBook(book);
            Database.Save();
            return book.Id;
        }
    }
}
