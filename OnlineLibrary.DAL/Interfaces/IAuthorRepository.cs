using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace OnlineLibrary.DAL.Interfaces
{
    public interface IAuthorRepository<T> where T : class
    {
        List<T> GetAllAuthors();
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="author"></param>
        /// <returns>id</returns>
        void CreateAuthor(T author);

        bool IsAuthorIdExists(int authorId);

        List<T> GetAuthorsByIdList(List<int> authorsId);

    }
}
