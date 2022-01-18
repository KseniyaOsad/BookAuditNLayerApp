using OnlineLibrary.Common.Entities;
using OnlineLibrary.BLL.Infrastructure;
using OnlineLibrary.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using OnlineLibrary.Common.Enums;

namespace OnlineLibrary.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthorController : Controller
    {
        private readonly IAuthorService<Author> _authorService;

        public AuthorController(IAuthorService<Author> iAuthor)
        {
            _authorService = iAuthor;
        }

        // POST:  api/Author/Create
        [HttpPost]
        public IActionResult Create([FromBody] Author author)
        {
            try
            {
                int? id = _authorService.CreateAuthor(author);
                ExceptionHelper.Check<Exception>(id == null || id == 0, "Id указан неправильно");
                return Ok(id);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }

        }

        // GET: api/Author/GetAllAuthors
        [HttpGet]
        public IActionResult GetAllAuthors()
        {
            try
            {
                List<Author> authors = _authorService.GetAllAuthors();
                ExceptionHelper.Check<Exception>(authors == null || !authors.Any(), "Авторов нет");
                return Ok(authors);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }
    }
}
