using OnlineLibrary.Common.Entities;
using OnlineLibrary.BLL.Infrastructure;
using OnlineLibrary.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLibrary.WEB.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthorController : Controller
    {
        private readonly IAuthorService<Author> _authorService;
        private readonly IDataExportService id;

        public AuthorController(IAuthorService<Author> iAuthor, IDataExportService id)
        {
            _authorService = iAuthor;
            this.id = id;
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
            try { 
                return Ok(_authorService.GetAllAuthors(id));
            }
            catch (ValidationException e)
            {
                return NotFound(e.Message);
            }
        }
    }
}
