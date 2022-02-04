using Dapper;
using Microsoft.Data.SqlClient;
using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.DAL.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLibrary.DAL.Repositories.Dapper
{
    public class AuthorDapperRepository : IAuthorRepository
    {
        private string _connectionString;

        public AuthorDapperRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<Author>> GetAllAuthorsAsync()
        {
            IEnumerable<Author> AuthorsAndBooks;
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT * 
                FROM Author AS A
                LEFT JOIN AuthorBook AS AB ON A.Id = AB.AuthorsId
                LEFT JOIN Book       AS B  ON B.Id = AB.BooksId";
                AuthorsAndBooks = await connection.QueryAsync<Author, Book, Author>(sql, (author, book) =>
                {
                    author.Books = book == null ? new List<Book>() : new List<Book>() { book };
                    return author;
                });
            }
            return AuthorsAndBooks
                .GroupBy(a => a.Id)
                .Select(group => {
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
            string sql = $"SELECT * FROM Author WHERE Id IN ({string.Join(",", authorsId)});";
            using (var connection = new SqlConnection(_connectionString))
            {
                return connection.Query<Author>(sql).ToList();
            }
        }

        public void InsertAuthor(Author author)
        {
            string sql = "INSERT INTO Author (Name) Values (@Name); SELECT SCOPE_IDENTITY();";

            using (var connection = new SqlConnection(_connectionString))
            {
                author.Id = connection.ExecuteScalar<int>(sql, author);
            }
        }
    }
}
