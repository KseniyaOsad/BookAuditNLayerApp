﻿using OnlineLibrary.Common.DBEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineLibrary.DAL.Interfaces
{
    public interface IAuthorRepository
    {
        Task<List<Author>> GetAllAuthorsAsync();

        void InsertAuthor(Author author);

        Task<List<Author>> GetAuthorsByIdListAsync(List<int> authorsId);

    }
}
