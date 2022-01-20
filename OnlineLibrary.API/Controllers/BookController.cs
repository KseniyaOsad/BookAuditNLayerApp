using Microsoft.AspNetCore.Mvc;
using System;
using OnlineLibrary.BLL.Interfaces;
using OnlineLibrary.Common.Entities;
using OnlineLibrary.Common.Helpers;
using OnlineLibrary.API.Model;
using AutoMapper;
using System.Collections.Generic;

namespace OnlineLibrary.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BookController : Controller
    {
        private readonly IBookService _bookService;

        private readonly IAuthorService _authorService;

        private readonly IMapper _mapper;

        public BookController(IBookService iBook, IAuthorService iAuthor, IMapper mapper)
        {
            _bookService = iBook;
            _authorService = iAuthor;
            _mapper = mapper;
        }

        // GET: api/Book/GetAllBooks
        [HttpGet]
        public IActionResult GetAllBooks()
        {
            try
            {
                return Ok(_bookService.GetAllBooks());
            }
            catch (Exception e)
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
            catch (Exception e)
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
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        // POST:  api/Book/Create
        [HttpPost]
        public IActionResult Create([FromBody] CreateBook cBook)
        {
            try
            {
                // необходимо настроить добавление с тегами!
                List<Author> authors = _authorService.GetAuthorsByIdList(cBook.Authors);

                Book book = _mapper.Map<CreateBook, Book>(cBook);
                book.Authors = authors;
                return Ok(_bookService.CreateBook(book));
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        // PUT:  api/Book/UpdateReservation/[id]
        [HttpPut("{id}")]
        public IActionResult UpdateReservation(int Id, [FromBody] Book book)
        {
            try
            {
                ExceptionHelper.Check<Exception>(Id != book.Id, "Id не совпадают");
                _bookService.ChangeBookReservation(Id, book.Reserve);
                Book b = _bookService.GetBookById(Id);
                return Ok(b);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        // PUT:  api/Book/UpdateArchievation/[id]
        [HttpPut("{id}")]
        public IActionResult UpdateArchievation(int Id, [FromBody] Book book)
        {
            try
            {
                ExceptionHelper.Check<Exception>(Id != book.Id, "Id не совпадают");
                _bookService.ChangeBookArchievation(Id, book.InArchive);
                Book b = _bookService.GetBookById(Id);
                return Ok(b);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        // GET: api/Book/GetAllGenres
        [HttpGet]
        public IActionResult GetAllGenres()
        {
             return Ok(Enum.GetNames(typeof(Genre)));
        }
    }
}
