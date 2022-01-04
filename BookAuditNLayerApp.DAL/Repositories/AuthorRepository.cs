
using BookAuditNLayer.GeneralClassLibrary.Entities;
using BookAuditNLayerApp.DAL.EF;
using BookAuditNLayerApp.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAuditNLayerApp.DAL.Repositories
{
    public class AuthorRepository : IAuthorRepository<Author>
    {
        BookContext _context;

        public AuthorRepository(BookContext context)
        {
            _context = context;
        }

        public int CreateAuthor(Author author)
        {
            _context.Add(author);
            _context.SaveChanges();
            return author.Id;
        }

        public List<Author> GetAllAuthors()
        {
            return _context.Author.OrderBy(a=>a.Name).ToList();
        }

        public bool IsAuthorIdExists(int authorId)
        {
            return _context.Author.Any(b => b.Id == authorId);
        }
    }
}
