using OnlineLibrary.Common.DBEntities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLibrary.BLL.Interfaces
{
    public interface IUserService
    {
        public Task<int> CreateUserAsync(User user);

        public Task<List<User>> GetAllUsersAsync();
    }
}
