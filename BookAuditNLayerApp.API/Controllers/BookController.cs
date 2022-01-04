using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using BookAuditNLayerApp.BLL.Interfaces;
using BookAuditNLayer.GeneralClassLibrary.Entities;
using BookAuditNLayerApp.BLL.Infrastructure;

namespace BookAuditNLayerApp.WEB.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BookController : Controller
    {
        private readonly IBookService<Book> _bookService;

        private readonly IAuthorService<Author> _authorService;

        public BookController(IBookService<Book> iBook, IAuthorService<Author> iAuthor)
        {
            _bookService = iBook;
            _authorService = iAuthor;
        }


        // GET: api/Book/GetAllBooks
        [HttpGet]
        public IActionResult GetAllBooks()
        {
            try
            {
                return Ok(_bookService.GetAllBooks());
            }
            catch (ValidationException e)
            {
                return NotFound(e.Message);
            }
            catch(Exception e)
            {
                return NotFound(e.Message);
            }
        }

        // GET: api/Book/GetBooksWithFilters
        [HttpGet("{authorId}/{name}/{reservation}/{inArchieve}")]
        public IActionResult GetBooksWithFilters(int? authorId, string name, int? reservation, int? inArchieve)
        {
            try
            {
                return Ok(_bookService.FilterBooks(authorId, name, reservation, inArchieve));
            }
            catch (ValidationException e)
            {
                return NotFound(e.Message);
            }
        }

        // GET: api/Book/GetBookById/[id]
        [HttpGet("{id}")]
        public IActionResult GetBookById(int? id)
        {
            try
            {
                return Ok(_bookService.GetBookById(id));
            }
            catch (ValidationException e)
            {
                return NotFound(e.Message);
            }
            catch(Exception e)
            {
                return NotFound(e.Message);
            }

        }

        // POST:  api/Book/Create
        [HttpPost]
        public IActionResult Create([Bind("Name,Description,AuthorId")] Book book)
        {
            try
            {
                if (_authorService.IsAuthorIdExists(book.AuthorId))
                {
                    int id = _bookService.CreateBook(book);
                    return Ok(id);
                }
                else
                {
                    return NotFound("Даного автора не существует");
                }
                
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }

        }
    }
}
