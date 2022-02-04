using Dapper;
using Microsoft.Data.SqlClient;
using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.DAL.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLibrary.DAL.Repositories.Dapper
{
    public class TagDapperRepository : ITagRepository
    {
        private string _connectionString;

        public TagDapperRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<Tag>> GetAllTagsAsync()
        {
            IEnumerable<Tag> TagsAndBooks;
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT * 
                FROM Tag AS T 
                LEFT JOIN BookTag AS BT ON T.Id = BT.TagsId
                LEFT JOIN Book    AS B  ON B.Id = BT.BooksId";
                TagsAndBooks = await connection.QueryAsync<Tag, Book, Tag>(sql, (tag, book) =>
                {
                    tag.Books = book == null ? new List<Book>() : new List<Book>() { book };
                    return tag;
                });
            }
            return TagsAndBooks
                .GroupBy(t=>t.Id)
                .Select(group => {
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
                string sql = $"SELECT * FROM Tag WHERE Id IN ({string.Join("," ,tagsId)});";
                return connection.Query<Tag>(sql).ToList();
            }
        }

        public void InsertTag(Tag tag)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "INSERT INTO Tag (Name) Values (@Name); SELECT SCOPE_IDENTITY();";
                tag.Id = connection.ExecuteScalar<int>(sql, tag);
            }
        } 
    }
}
