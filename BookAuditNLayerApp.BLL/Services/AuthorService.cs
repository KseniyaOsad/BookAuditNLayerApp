using BookAuditNLayer.GeneralClassLibrary.Entities;
using BookAuditNLayer.GeneralClassLibrary.Enums;
using BookAuditNLayerApp.BLL.Infrastructure;
using BookAuditNLayerApp.BLL.Interfaces;
using BookAuditNLayerApp.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookAuditNLayerApp.BLL.Services
{
    public class AuthorService : IAuthorService<Author>
    {
        IUnitOfWork Database { get; set; }
        public AuthorService(IUnitOfWork uow)
        {
            Database = uow;
        }

        public int CreateAuthor(Author author)
        {
            return Database.Author.CreateAuthor(author);
        }

        public List<Author> GetAllAuthors()
        {
            List<Author> authors = Database.Author.GetAllAuthors();
            if (authors.Count == 0 || authors == null)
            {
                throw new ValidationException("Авторов не существует", ErrorList.ListIsEmpty);
            }
            return authors;
        }

        public bool IsAuthorIdExists(int? authorId)
        {
            if (authorId == null || authorId <= 0)
            {
                throw new ValidationException("AuthorId указан неправильно", ErrorList.IncorrectId);
            }
            try
            {
                return Database.Author.IsAuthorIdExists((int)authorId);
            }
            catch (Exception e)
            {
                throw new ValidationException(e.Message, ErrorList.NotFound);
            }
        }
    }
}
