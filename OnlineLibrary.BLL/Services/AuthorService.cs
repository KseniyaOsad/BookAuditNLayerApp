using OnlineLibrary.Common.Entities;
using OnlineLibrary.Common.Helpers;
using OnlineLibrary.BLL.Interfaces;
using OnlineLibrary.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using OnlineLibrary.Common.Validators;
using OnlineLibrary.Common.Exceptions;
using OnlineLibrary.Common.Exceptions.Enum;

namespace OnlineLibrary.BLL.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly AuthorValidator authorValidator;

        public AuthorService(IUnitOfWork uow)
        {
            unitOfWork = uow;
            authorValidator = new AuthorValidator();
        }

        public int CreateAuthor(Author author)
        {
            unitOfWork.AuthorRepository.InsertAuthor(author);
            unitOfWork.Save();
            ExceptionHelper.Check<OLException>(author.Id == 0, "Автор не был создан", ExceptionType.BadRequest);
            return author.Id;
        }

        public List<Author> GetAllAuthors()
        {
            List<Author> authors = unitOfWork.AuthorRepository.GetAllAuthors();
            return authors;
        }

        public List<Author> GetAuthorsByIdList(List<int> authorsId)
        {
            List<Author> authors = unitOfWork.AuthorRepository.GetAuthorsByIdList(authorsId);
            ExceptionHelper.Check<OLException>(authors == null || !authors.Any(), "Авторы не найдены", ExceptionType.NotFound);
            return authors;
        }
    }
}
