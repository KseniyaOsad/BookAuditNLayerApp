using OnlineLibrary.Common.Entities;
using OnlineLibrary.Common.Enums;
using OnlineLibrary.BLL.Infrastructure;
using OnlineLibrary.BLL.Interfaces;
using OnlineLibrary.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineLibrary.BLL.Services
{
    public class BookService : IBookService<Book>
    {
        private readonly IUnitOfWork unitOfWork;

        public BookService(IUnitOfWork uow)
        {
            unitOfWork = uow;
        }

        public List<Book> GetAllBooks()
        {
            List<Book> books = unitOfWork.Book.GetAllBooks();
            ExceptionHelper.Check<Exception>(books == null || !books.Any(), "Книг нет");
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
                books = unitOfWork.Book.FilterBooks((int)authorId, name);
            }
            // Заполнен только автор книги.
            else if (authorId != null)
            {
                books = unitOfWork.Book.FilterBooks((int)authorId);
            }
            // Заполнено только название книги.
            else if (!String.IsNullOrEmpty(name))
            {
                books = unitOfWork.Book.FilterBooks(name);
            }
            // Поля фильтрации пустые, получаем весь список.
            else
            {
                books = unitOfWork.Book.GetAllBooks();
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
            ExceptionHelper.Check<Exception>(books == null || books.Any(), "Книг по данному запросу нет");
            return books;
        }


        public Book GetBookById(int? bookId)
        {
            ExceptionHelper.Check<Exception>(bookId == null || bookId <= 0, "Id указан неправильно");
            Book book = unitOfWork.Book.GetBookById((int)bookId);
            ExceptionHelper.Check<Exception>(book == null, "Книга не найдена");
            return book;
        }

        public void ChangeBookReservation(int? bookId, bool newReservationValue)
        {
            ExceptionHelper.Check<Exception>(bookId == null || bookId <= 0, "Id указан неправильно");
            ExceptionHelper.Check<Exception>(!unitOfWork.Book.IsBookIdExists((int)bookId), "Книга не найдена");
            unitOfWork.Book.ChangeBookReservation((int)bookId, newReservationValue);
            unitOfWork.Save();
        }

        public void ChangeBookArchievation(int? bookId, bool newArchievationValue)
        {
            ExceptionHelper.Check<Exception>(bookId == null || bookId <= 0, "Id указан неправильно");
            ExceptionHelper.Check<Exception>(!unitOfWork.Book.IsBookIdExists((int)bookId), "Книга не найдена");
            unitOfWork.Book.ChangeBookArchievation((int)bookId, newArchievationValue);
            unitOfWork.Save();
        }

        public int CreateBook(Book book)
        {
            ExceptionHelper.Check<Exception>(book.Name == null || book.Name.Trim() == "", "Поле 'Имя' заполнено неверно");
            ExceptionHelper.Check<Exception>(book.Description == null || book.Description.Trim() == "", "Поле 'Описание' заполнено неверно");
            unitOfWork.Book.CreateBook(book);
            unitOfWork.Save();
            return book.Id;
        }
    }
}
