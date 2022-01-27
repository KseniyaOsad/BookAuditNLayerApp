using OnlineLibrary.Common.DBEntities;
using System.Collections.Generic;

namespace OnlineLibrary.DAL.Interfaces
{
    public interface IAuthorRepository
    {
        List<Author> GetAllAuthors();

        void InsertAuthor(Author author);

        List<Author> GetAuthorsByIdList(List<int> authorsId);

    }
}
