using OnlineLibrary.Common.Entities;
using OnlineLibrary.Common.Enums;
using OnlineLibrary.BLL.Infrastructure;
using OnlineLibrary.BLL.Interfaces;
using OnlineLibrary.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace OnlineLibrary.BLL.Services
{
    public class AuthorService : IAuthorService<Author>
    {
        private readonly IDataExportService _dataExportService;

        IUnitOfWork Database { get; set; }
        public AuthorService(IUnitOfWork uow, IDataExportService _dataExportService)
        {
            Database = uow;
            this._dataExportService = _dataExportService;
        }

        public int CreateAuthor(Author author)
        {
            Database.Author.CreateAuthor(author);
            Database.Save();
            return author.Id;
        }

        public List<Author> GetAllAuthors(IDataExportService dataExportService)
        {
            var isSame = dataExportService == _dataExportService;

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

        public bool IsAuthorsExists(List<int> authorsId)
        {
            if (!authorsId.Any())
            {
                throw new ValidationException("Авторы не указаны", ErrorList.FieldIsIncorrect);
            }
            try
            {
                foreach (var author in authorsId)
                {
                    if (!Database.Author.IsAuthorIdExists(author))
                        throw new ValidationException($"Автор {author} не найден", ErrorList.NotFound);
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Author> GetAuthorsByIdList(List<int> authorsId)
        {
            IsAuthorsExists(authorsId);
            return Database.Author.GetAuthorsByIdList(authorsId);
        }
    }
}
