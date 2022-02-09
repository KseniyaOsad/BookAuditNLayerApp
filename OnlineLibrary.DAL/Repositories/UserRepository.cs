using Dapper;
using Microsoft.Data.SqlClient;
using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.DAL.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLibrary.DAL.Repositories.Dapper
{
    public class UserRepository : IUserRepository
    {
        private string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task CreateUserAsync(User user)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "INSERT INTO Users (Name, Email, DateOfBirth, RegistrationDate) Values (@Name, @Email, @DateOfBirth, @RegistrationDate);  SELECT SCOPE_IDENTITY();";
                user.Id = await connection.ExecuteScalarAsync<int>(sql, user);
            }
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, Name, Email, RegistrationDate, DateOfBirth FROM Users";
                return (await connection.QueryAsync<User>(sql)).ToList();
            }
        }

        public async Task<bool> IsUserExistAsync(int userId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return (await connection.QueryAsync<object>(
                    "SELECT 1 FROM Users WHERE Id = @Id;", new { Id = userId }))
                    .Any();
            }
        }
    }
}
