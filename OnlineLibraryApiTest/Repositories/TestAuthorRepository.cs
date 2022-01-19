using OnlineLibrary.Common.Entities;
using OnlineLibrary.DAL.Interfaces;
using System;
using System.Collections.Generic;

namespace OnlineLibraryApiTest.Repositories
{
    public class TestAuthorRepository : IAuthorRepository<Author>
    {
        public void CreateAuthor(Author author)
        {
        }

        public List<Author> GetAllAuthors()
        {
            throw new NotImplementedException();
        }

        public List<Author> GetAuthorsByIdList(List<int> authorsId)
        {
            throw new NotImplementedException();
        }

    }
}
