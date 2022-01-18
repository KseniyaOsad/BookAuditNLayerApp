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
    public class AuthorService : IAuthorService<Author>
    {
        private readonly IUnitOfWork unitOfWork;

        public AuthorService(IUnitOfWork uow)
        {
            unitOfWork = uow;
        }

        public int CreateAuthor(Author author)
        {
            ExceptionHelper.Check<Exception>(author.Name == null || author.Name.Trim() == "", "Поле указано неверно");
            unitOfWork.Author.CreateAuthor(author);
            unitOfWork.Save();
            return author.Id;
        }

        public List<Author> GetAllAuthors()
        {
            List<Author> authors = unitOfWork.Author.GetAllAuthors();
            ExceptionHelper.Check<Exception>(!authors.Any(), "Авторов не существует");
            return authors;
        }

        public bool IsAuthorIdExists(params int[] authorId)
        {
            ExceptionHelper.Check<Exception>(authorId == null || !authorId.Any(), "Авторы не указаны");

            try
            {
                foreach (var author in authorId)
                {
                    ExceptionHelper.Check<Exception>(!unitOfWork.Author.IsAuthorIdExists(author), $"Автор {author} не найден");
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Author> GetAuthorsByIdList(List<int> authorsId)
        {
            List<Author> authors = unitOfWork.Author.GetAuthorsByIdList(authorsId);
            ExceptionHelper.Check<Exception>(authors == null || !authors.Any(), "Авторы не найдены");
            return authors;
        }
    }
}
