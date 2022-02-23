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
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IOptions<DBConnection> connOptions)
        {
            _connectionString = connOptions.Value.BookContext;
        }

        public async Task CreateUserAsync(User user)
        {
            using (var connection = new SqlConnection(_connectionString))
                user.Id = await connection.ExecuteScalarAsync<int>("sp_CreateUser", 
                    new { name = user.Name, email = user.Email, dateOfBirth = user.DateOfBirth }, 
                    commandType: CommandType.StoredProcedure);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
                return (await connection.QueryAsync<User>("sp_GetAllUsers", 
                    commandType: CommandType.StoredProcedure)).ToList();
        }

        public async Task<bool> IsUserExistAsync(int userId)
        {
            using (var connection = new SqlConnection(_connectionString))
                return (await connection.QueryAsync<User>(
                    "sp_GetUserById", new { id = userId },
                    commandType: CommandType.StoredProcedure))
                    .Any();
        }

        public async Task<List<User>> GetUsersByIdListAsync(List<int> usersId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                List<IdList> idLists = usersId.Select(x => new IdList(x)).ToList();
                var parameters = new DynamicParameters();
                parameters.AddTable("@ids", "t_IdList", idLists);

                return (await connection.QueryAsync<User>("sp_GetUsersByIdList",
                    parameters,
                    commandType: CommandType.StoredProcedure)).ToList();
            }
        }
    }
}
