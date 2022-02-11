using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using OnlineLibrary.Common.Connection;
using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.DAL.Interfaces;
using System.Collections.Generic;
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
            {
                string sql = @"SELECT A.Id, A.Name, B.Id, B.Name, B.Description, B.Genre , B.InArchive , B.RegistrationDate, AB.AuthorsId, AB.BooksId
                FROM Authors AS A
                LEFT JOIN AuthorBook  AS AB ON A.Id = AB.AuthorsId
                LEFT JOIN Books       AS B  ON B.Id = AB.BooksId";
                AuthorsAndBooks = await connection.QueryAsync<Author, Book, Author>(sql, (author, book) =>
                {
                    author.Books = book == null ? new List<Book>() : new List<Book>() { book };
                    return author;
                });
            }
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
            string sql = "SELECT Id, Name FROM Authors WHERE Id IN @Ids;";
            using (var connection = new SqlConnection(_connectionString))
            {
                return (await connection.QueryAsync<Author>(sql, new { Ids = authorsId })).ToList();
            }
        }

        public async Task CreateAuthorAsync(Author author)
        {
            string sql = "INSERT INTO Authors (Name) Values (@Name); SELECT SCOPE_IDENTITY();";

            using (var connection = new SqlConnection(_connectionString))
            {
                author.Id = await connection.ExecuteScalarAsync<int>(sql, author);
            }
        }
    }
}
