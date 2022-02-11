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
    public class TagRepository : ITagRepository
    {
        private readonly string _connectionString;

        public TagRepository(IOptions<DBConnection> connOptions)
        {
            _connectionString = connOptions.Value.BookContext;
        }

        public async Task<List<Tag>> GetAllTagsAsync()
        {
            IEnumerable<Tag> TagsAndBooks;
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT T.Id, T.Name, B.Id, B.Name, B.Description, B.Genre , B.InArchive , B.RegistrationDate, BT.TagsId, BT.BooksId
                FROM Tags AS T 
                LEFT JOIN BookTag  AS BT ON T.Id = BT.TagsId
                LEFT JOIN Books    AS B  ON B.Id = BT.BooksId";
                TagsAndBooks = await connection.QueryAsync<Tag, Book, Tag>(sql, (tag, book) =>
                {
                    tag.Books = book == null ? new List<Book>() : new List<Book>() { book };
                    return tag;
                });
            }
            return TagsAndBooks
                .GroupBy(t => t.Id)
                .Select(group =>
                {
                    var tag = group.First();
                    if (group.Count() > 1)
                    {
                        tag.Books = group.Select(t => t.Books.Single()).ToList();
                    }
                    return tag;
                })
                .ToList();
        }

        public async Task<List<Tag>> GetTagsByIdListAsync(List<int> tagsId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT Id, Name FROM Tags WHERE Id IN @Ids";
                return (await connection.QueryAsync<Tag>(sql, new { Ids = tagsId })).ToList();
            }
        }

        public async Task CreateTagAsync(Tag tag)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "INSERT INTO Tags (Name) Values (@Name); SELECT SCOPE_IDENTITY();";
                tag.Id = await connection.ExecuteScalarAsync<int>(sql, tag);
            }
        }
    }
}
