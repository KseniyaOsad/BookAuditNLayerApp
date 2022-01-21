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
            ExceptionHelper.Check<OLBadRequest>(author == null, "A null object came to the method");
            unitOfWork.AuthorRepository.InsertAuthor(author);
            unitOfWork.Save();
            ExceptionHelper.Check<OLBadRequest>(author.Id == 0, "The author was not created");
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
            ExceptionHelper.Check<OLNotFound>(authors == null || !authors.Any(), "Authors not found");
            return authors;
        }
    }
}
