using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace BookAuditNLayerApp.DAL.Interfaces
{
    public interface IAuthorRepository<T> where T : class
    {
        List<T> GetAllAuthors();

        int CreateAuthor(T author);
        // Return id.
    }
}
