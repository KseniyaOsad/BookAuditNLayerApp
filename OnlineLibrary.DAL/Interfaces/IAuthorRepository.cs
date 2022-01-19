﻿using System.Collections.Generic;

namespace OnlineLibrary.DAL.Interfaces
{
    public interface IAuthorRepository<T> where T : class
    {
        List<T> GetAllAuthors();

        void CreateAuthor(T author);

        List<T> GetAuthorsByIdList(List<int> authorsId);

    }
}
