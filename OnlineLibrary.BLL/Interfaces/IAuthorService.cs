using System.Collections.Generic;

namespace OnlineLibrary.BLL.Interfaces
{
    public interface IAuthorService<T> where T : class
    {
        List<T> GetAllAuthors();

        int CreateAuthor(T author);
        // Returns Id.

        List<T> GetAuthorsByIdList(List<int> authorsId);

    }
}
