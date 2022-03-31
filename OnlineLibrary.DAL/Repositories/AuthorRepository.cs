using Dapper;
using DapperParameters;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using OnlineLibrary.Common.Connection;
using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.DAL.DTO;
using OnlineLibrary.DAL.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLibrary.DAL.Repositories.Dapper
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly string _connectionString;

        public AuthorRepository(IOptions<DBConnection> connOptions)
        {
            _connectionString = connOptions.Value.BookContext;
        }

        public async Task<List<Author>> GetAllAuthorsAsync()
        {
            IEnumerable<Author> AuthorsAndBooks;
            using (var connection = new SqlConnection(_connectionString))
                AuthorsAndBooks = await connection.QueryAsync<Author, Book, Author>("sp_GetAllAuthors", (author, book) =>
                {
                    author.Books = book == null ? new List<Book>() : new List<Book>() { book };
                    return author;
                }, commandType: CommandType.StoredProcedure);

            return AuthorsAndBooks
                .GroupBy(a => a.Id)
                .Select(group =>
                {
                    var author = group.First();
                    if (group.Count() > 1)
                    {
                        author.Books = group.Select(a => a.Books.Single()).ToList();
                    }
                    return author;
                })
                .ToList();
        }

        public async Task<List<Author>> GetAuthorsByIdListAsync(List<int> authorsId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                List<IdList> idLists = authorsId.Select(x => new IdList(x)).ToList();
                var parameters = new DynamicParameters();
                parameters.AddTable("@ids", "t_IdList", idLists);
                return (await connection.QueryAsync<Author>("sp_GetAuthorsByIdList",
                    parameters,
                    commandType: CommandType.StoredProcedure)).ToList();
            }
        }

        public async Task CreateAuthorAsync(Author author)
        {
            using (var connection = new SqlConnection(_connectionString))
                author.Id = await connection.ExecuteScalarAsync<int>("sp_CreateAuthor", new { name = author.Name }, 
                    commandType: CommandType.StoredProcedure);
        }
    }
}
