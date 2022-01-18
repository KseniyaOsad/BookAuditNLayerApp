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
using OnlineLibrary.API.Model;
using OnlineLibrary.API.Helper;
using OnlineLibrary.Common.Enums;

namespace OnlineLibrary.API.Controllers
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
                List<Book> books = _bookService.GetAllBooks();
                if (books == null || !books.Any()) throw new ValidationException("Книг нет", ErrorList.ListIsEmpty);
                return Ok(books);
            }
            catch (ValidationException e)
            {
                return NotFound(e.Message);
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
                List<Book> books = _bookService.FilterBooks(authorId, name, reservation, inArchieve);
                if (books == null || !books.Any()) throw new ValidationException("Книг нет", ErrorList.ListIsEmpty);
                return Ok(books);
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
                Book book = _bookService.GetBookById(id);
                if (book == null) throw new ValidationException("Книги нет", ErrorList.NotFound);
                return Ok(book);
            }
            catch (ValidationException e)
            {
                return NotFound(e.Message);
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
                // необходимо добавление создания связи
                List<Author> authors = _authorService.GetAuthorsByIdList(cBook.Authors);
                if (authors == null || !authors.Any()) throw new ValidationException("Авторов нет", ErrorList.ListIsEmpty);
                int? id = _bookService.CreateBook(ParseCreateBookModel.CreateBookToBook(cBook, authors));
                if (id == null || id == 0) throw new Exception("Книга не была создана");
                return Ok(id);

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
                if (Id == book.Id)
                {
                    _bookService.ChangeBookReservation(Id, book.Reserve);
                    Book b = _bookService.GetBookById(Id);
                    if (b == null) throw new ValidationException("Книги нет", ErrorList.NotFound);
                    return Ok(b);
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
        public IActionResult UpdateArchievation(int Id, [FromBody] Book book)
        {
            try
            {
                if (Id == book.Id)
                {
                    _bookService.ChangeBookArchievation(Id, book.InArchive);
                    Book b = _bookService.GetBookById(Id);
                    if (b == null) throw new ValidationException("Книги нет", ErrorList.NotFound);
                    return Ok(b);
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
