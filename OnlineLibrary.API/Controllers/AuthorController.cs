using OnlineLibrary.Common.Entities;
using OnlineLibrary.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace OnlineLibrary.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
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
            try
            {
                int id = _authorService.CreateAuthor(author);
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
                return Ok(authors);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }
    }
}
