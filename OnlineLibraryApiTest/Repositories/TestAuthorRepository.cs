using OnlineLibrary.Common.Entities;
using OnlineLibrary.DAL.Interfaces;
using System;
using System.Collections.Generic;

namespace OnlineLibraryApiTest.Repositories
{
    public class TestAuthorRepository : IAuthorRepository
    {
        public void InsertAuthor(Author author)
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
