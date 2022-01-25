using OnlineLibrary.Common.Entities;
using OnlineLibrary.Common.Helpers;
using OnlineLibrary.BLL.Interfaces;
using OnlineLibrary.DAL.Interfaces;
using System.Collections.Generic;
using System.Linq;
using OnlineLibrary.Common.Validators;
using OnlineLibrary.Common.Exceptions;

namespace OnlineLibrary.BLL.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthorService(IUnitOfWork uow)
        {
            _unitOfWork = uow;
        }

        public int CreateAuthor(Author author)
        {
            ExceptionHelper.Check<OLBadRequest>(author == null, "A null object came to the method");
            _unitOfWork.AuthorRepository.InsertAuthor(author);
            _unitOfWork.Save();
            ExceptionHelper.Check<OLBadRequest>(author.Id == 0, "The author was not created");
            return author.Id;
        }

        public List<Author> GetAllAuthors()
        {
            List<Author> authors = _unitOfWork.AuthorRepository.GetAllAuthors();
            return authors;
        }

        public List<Author> GetAuthorsByIdList(List<int> authorsId)
        {
            List<Author> authors = _unitOfWork.AuthorRepository.GetAuthorsByIdList(authorsId);
            ExceptionHelper.Check<OLNotFound>(authors == null || !authors.Any(), "Authors not found");
            return authors;
        }
    }
}
