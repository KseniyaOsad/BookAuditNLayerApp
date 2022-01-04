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
    public class AuthorController : Controller
    {
        private readonly IAuthorService<Author> _authorService;

        public AuthorController(IAuthorService<Author> iAuthor)
        {
            _authorService = iAuthor;
        }

        //// POST: api/author/create
        //[HttpPost("Create")]
        //public IActionResult Create([Bind("Name")] Author author)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _authorService.CreateAuthor(author);
        //        return RedirectToAction("Create", "Book");
        //    }
        //    return View();

        //}

        // GET: api/author/all
        [HttpGet("All")]
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
