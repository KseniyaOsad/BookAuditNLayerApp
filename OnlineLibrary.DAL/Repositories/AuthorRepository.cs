using Microsoft.EntityFrameworkCore;
using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.DAL.EF;
using OnlineLibrary.DAL.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLibrary.DAL.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        BookContext _context;

        public AuthorRepository(BookContext context)
        {
            _context = context;
        }

        public void InsertAuthor(Author author)
        {
            _context.Add(author);
        }

        public  Task<List<Author>> GetAllAuthorsAsync()
        {
            return  _context.Author
                .Include(a => a.Books)
                .OrderBy(a => a.Name).ToListAsync();
        }

        public Task<List<Author>> GetAuthorsByIdListAsync(List<int> authorsId)
        {
            return _context.Author.Where(a => authorsId.Contains(a.Id)).ToListAsync();
        }

    }
}
