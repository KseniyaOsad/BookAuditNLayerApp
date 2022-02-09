﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnlineLibrary.API.Filters;
using OnlineLibrary.API.Model;
using OnlineLibrary.BLL.Interfaces;
using OnlineLibrary.Common.DBEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineLibrary.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    [TypeFilter(typeof(GenericExceptionFilter))]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        private readonly ILogger<UserController> _logger;

        private readonly IMapper _mapper;

        public UserController(IUserService iUser, IMapper mapper, ILogger<UserController> logger)
        {
            _userService = iUser;
            _logger = logger;
            _mapper = mapper;
        }

        // POST:  api/users
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateUser cUser)
        {
            User user = _mapper.Map<CreateUser, User>(cUser);
            _logger.LogInformation($"Map CreateUser to User");
            int id = await _userService.CreateUserAsync(user);
            _logger.LogInformation($"New user created. User ID = {id}.");
            return Ok(id);
        }

        // GET: api/users
        [HttpGet]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            List<User> users = await _userService.GetAllUsersAsync();
            _logger.LogInformation($"Getting all users. Users count = {users?.Count}.");
            return Ok(users);
        }
    }
}
