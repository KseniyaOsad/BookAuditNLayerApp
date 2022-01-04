using BookAuditNLayer.GeneralClassLibrary.Entities;
using BookAuditNLayerApp.BLL.Infrastructure;
using BookAuditNLayerApp.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAuditNLayerApp.WEB.Controllers
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
        public IActionResult Create([Bind("Name")] Author author)
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
                return Ok(_authorService.GetAllAuthors());
            }
            catch (ValidationException e)
            {
                return NotFound(e.Message);
            }
        }
    }
}
