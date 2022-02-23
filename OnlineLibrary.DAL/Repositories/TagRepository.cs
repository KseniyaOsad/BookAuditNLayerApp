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
                TagsAndBooks = await connection.QueryAsync<Tag, Book, Tag>("sp_GetAllTags", (tag, book) =>
                {
                    tag.Books = book == null ? new List<Book>() : new List<Book>() { book };
                    return tag;
                }, commandType: CommandType.StoredProcedure);
            
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
                List<IdList> idLists = tagsId.Select(x=>new IdList(x)).ToList();
                var parameters = new DynamicParameters();
                parameters.AddTable("@ids", "t_IdList", idLists);

                return (await connection.QueryAsync<Tag>("sp_GetTagsByIdList",
                    parameters,
                    commandType: CommandType.StoredProcedure)).ToList();
            }
        }

        public async Task CreateTagAsync(Tag tag)
        {
            using (var connection = new SqlConnection(_connectionString))
                tag.Id = await connection.ExecuteScalarAsync<int>("sp_CreateTag", 
                    new { name = tag.Name },
                    commandType: CommandType.StoredProcedure);
        }
    }
}
