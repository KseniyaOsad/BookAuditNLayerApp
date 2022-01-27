using OnlineLibrary.Common.DBEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineLibrary.BLL.Interfaces
{
    public interface IAuthorService
    {
        Task<List<Author>> GetAllAuthorsAsync();

        Task<int> CreateAuthorAsync(Author author);

        Task<List<Author>> GetAuthorsByIdListAsync(List<int> authorsId);

    }
}
