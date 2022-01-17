using OnlineLibrary.API.Model;
using OnlineLibrary.BLL.Infrastructure;
using OnlineLibrary.Common.Entities;
using OnlineLibrary.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLibrary.API.Helper
{
    public static class ParseCreateBookModel
    {
        public static Book CreateBookToBook(CreateBook createBook, List<Author> authors)
        {
            if (!Enum.IsDefined(typeof(Genre), createBook.Genre))
                throw new ValidationException("Даного жарна не существует", ErrorList.FieldIsIncorrect);
            return new Book() {
                Name = createBook.Name,
                Description = createBook.Description,
                Reserve = false,
                InArchive = false,
                Genre = (Genre)createBook.Genre,
                RegistrationDate = DateTime.Now,
                Authors = authors
            };
        }
    }
}
