﻿using OnlineLibrary.BLL.Interfaces;
using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.Common.Exceptions;
using OnlineLibrary.Common.Extensions;
using OnlineLibrary.DAL.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineLibrary.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork uow)
        {
            _unitOfWork = uow;
        }

        public async Task<int> CreateUserAsync(User user)
        {
            await _unitOfWork.UserRepository.CreateUserAsync(user);
            ExceptionExtensions.Check<OLBadRequest>(user.Id == 0, "The user was not created");
            return user.Id;
        }

        public Task<List<User>> GetAllUsersAsync()
        {
            return _unitOfWork.UserRepository.GetAllUsersAsync();
        }
    }
}
