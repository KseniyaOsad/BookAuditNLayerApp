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
            if (author.Name == null || author.Name.Trim() == "")
                throw new ValidationException("Поле указано неверно", ErrorList.FieldIsIncorrect);
            unitOfWork.Author.CreateAuthor(author);
            unitOfWork.Save();
            return author.Id;
        }

        public List<Author> GetAllAuthors()
        {
            List<Author> authors = unitOfWork.Author.GetAllAuthors();
            if (!authors.Any())
            {
                throw new ValidationException("Авторов не существует", ErrorList.ListIsEmpty);
            }
            return authors;
        }

        public bool IsAuthorIdExists(params int[] authorId)
        {
            if (!authorId.Any())
            {
                throw new ValidationException("Авторы не указаны", ErrorList.FieldIsIncorrect);
            }

            try
            {
                foreach (var author in authorId)
                {
                    if (!unitOfWork.Author.IsAuthorIdExists(author))
                        throw new ValidationException($"Автор {author} не найден", ErrorList.NotFound);
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
            if (authors == null || !authors.Any())
                throw new ValidationException($"Авторы не найдены", ErrorList.NotFound);
            return authors;
        }
    }
}
