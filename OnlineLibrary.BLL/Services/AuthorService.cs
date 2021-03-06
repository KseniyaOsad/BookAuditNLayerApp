using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.Common.Extensions;
using OnlineLibrary.BLL.Interfaces;
using OnlineLibrary.DAL.Interfaces;
using System.Collections.Generic;
using System.Linq;
using OnlineLibrary.Common.Exceptions;
using System.Threading.Tasks;

namespace OnlineLibrary.BLL.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthorService(IUnitOfWork uow)
        {
            _unitOfWork = uow;
        }

        public async Task<int> CreateAuthorAsync(Author author)
        {
            await _unitOfWork.AuthorRepository.CreateAuthorAsync(author);
            ExceptionExtensions.Check<OLBadRequest>(author.Id == 0, "The author was not created");
            return author.Id;
        }

        public Task<List<Author>> GetAllAuthorsAsync()
        {
            return _unitOfWork.AuthorRepository.GetAllAuthorsAsync();
        }

        public async Task<List<Author>> GetAuthorsByIdListAsync(List<int> authorsId)
        {
            List<Author> authors = await _unitOfWork.AuthorRepository.GetAuthorsByIdListAsync(authorsId);
            ExceptionExtensions.Check<OLNotFound>(!authors.Any(), "Authors not found");
            return authors;
        }
    }
}
