using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineLibrary.BLL.Interfaces;
using OnlineLibrary.Common.Entities;
using OnlineLibrary.BLL.Infrastructure;
using OnlineLibrary.BLL.Model;
using OnlineLibrary.API.Model;
using OnlineLibrary.API.Helper;

namespace OnlineLibrary.WEB.Controllers
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
        public IActionResult Create([Bind("Name,Description,Genre,Authors")] CreateBook cBook)
        {
            try
            {
                // необходимо добавление создания связи
                List<Author> authors = _authorService.GetAuthorsByIdList(cBook.Authors);
                int id = _bookService.CreateBook(ParseCreateBookModel.CreateBookToBook(cBook, authors));
                return Ok(id);

            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }

        }

        // PUT:  api/Book/UpdateReservation/[id]
        [HttpPut("{id}")]
        public IActionResult UpdateReservation(int Id, [Bind("Id,Reserve")] Book book)
        {
            try
            {
                if (Id == book.Id )
                {
                    _bookService.ChangeBookReservation(Id, book.Reserve);
                    return Ok(_bookService.GetBookById(Id));
                }
                else
                {
                    return NotFound("Id не совпадают");
                }

            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        // PUT:  api/Book/UpdateArchievation/[id]
        [HttpPut("{id}")]
        public IActionResult UpdateArchievation(int Id, [Bind("Id,InArchive")] Book book)
        {
            try
            {
                if (Id == book.Id)
                {
                    _bookService.ChangeBookArchievation(Id, book.InArchive);
                    return Ok(_bookService.GetBookById(Id));
                }
                else
                {
                    return NotFound("Id не совпадают");
                }

            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }

        }
    }
}
