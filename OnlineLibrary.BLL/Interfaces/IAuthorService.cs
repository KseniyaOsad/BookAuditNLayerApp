using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineLibrary.BLL.Interfaces
{
    public interface IAuthorService<T> where T : class
    {
        List<T> GetAllAuthors(IDataExportService dataExportService);

        int CreateAuthor(T author);
        // Returns Id.

        bool IsAuthorIdExists(int? authorId);

        bool IsAuthorsExists(List<int> authorsId);

        List<T> GetAuthorsByIdList(List<int> authorsId);

    }
}
