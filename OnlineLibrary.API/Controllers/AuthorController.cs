﻿using OnlineLibrary.Common.Entities;
using OnlineLibrary.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using OnlineLibrary.Common.Filters;

namespace OnlineLibrary.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [TypeFilter(typeof(GenericExceptionFilter))]
    public class AuthorController : Controller
    {
        private readonly IAuthorService _authorService;

        public AuthorController(IAuthorService iAuthor)
        {
            _authorService = iAuthor;
        }

        // POST:  api/Author/Create
        [HttpPost]
        public IActionResult Create([FromBody] Author author)
        {
            int id = _authorService.CreateAuthor(author);
            return Ok(id);
        }

        // GET: api/Author/GetAllAuthors
        [HttpGet]
        public IActionResult GetAllAuthors()
        {
            List<Author> authors = _authorService.GetAllAuthors();
            return Ok(authors);
        }
    }
}
