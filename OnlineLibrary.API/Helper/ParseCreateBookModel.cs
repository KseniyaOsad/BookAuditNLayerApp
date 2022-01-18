using OnlineLibrary.API.Model;
using OnlineLibrary.BLL.Infrastructure;
using OnlineLibrary.Common.Entities;
using OnlineLibrary.Common.Enums;
using System;
using System.Collections.Generic;

namespace OnlineLibrary.API.Helper
{
    public static class ParseCreateBookModel
    {
        public static Book CreateBookToBook(CreateBook createBook, List<Author> authors)
        {
            ExceptionHelper.Check<Exception>(createBook.Genre == null || !Enum.IsDefined(typeof(Genre), createBook.Genre), "Даного жарна не существует");
            return new Book()
            {
                Name = createBook.Name,
                Description = createBook.Description,
                Reserve = false,
                InArchive = false,
                Genre = (Genre)(int)createBook.Genre,
                RegistrationDate = DateTime.Now,
                Authors = authors
            };
        }
    }
}
