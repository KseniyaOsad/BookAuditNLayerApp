using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using OnlineLibrary.Common.Connection;
using OnlineLibrary.Common.DBEntities;
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
    }
}
