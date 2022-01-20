using OnlineLibrary.Common.Entities;
using System.Collections.Generic;

namespace OnlineLibrary.BLL.Interfaces
{
    public interface IAuthorService
    {
        List<Author> GetAllAuthors();

        int CreateAuthor(Author author);
        // Returns Id.

        List<Author> GetAuthorsByIdList(List<int> authorsId);

    }
}
