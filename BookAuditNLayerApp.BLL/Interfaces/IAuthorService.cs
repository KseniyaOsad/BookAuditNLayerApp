using System;
using System.Collections.Generic;
using System.Text;

namespace BookAuditNLayerApp.BLL.Interfaces
{
    public interface IAuthorService<T> where T : class
    {
        List<T> GetAllAuthors();

        int CreateAuthor(T author);
        // Returns Id.
    }
}
