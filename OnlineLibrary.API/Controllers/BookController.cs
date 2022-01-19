using Microsoft.AspNetCore.Mvc;
using System;
using OnlineLibrary.BLL.Interfaces;
using OnlineLibrary.Common.Entities;
using OnlineLibrary.Common.Helpers;
using OnlineLibrary.API.Model;
using AutoMapper;

namespace OnlineLibrary.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BookController : Controller
    {
        private readonly IBookService _bookService;

        private readonly IAuthorService _authorService;

        public BookController(IBookService iBook, IAuthorService iAuthor)
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
                ExceptionHelper.Check<Exception>(cBook.Genre == null || !Enum.IsDefined(typeof(Genre), cBook.Genre), "Даного жарна не существует");
                // Config mapper.
                var config = new MapperConfiguration(cfg => cfg.CreateMap<CreateBook, Book>()
                .ForMember(dest => dest.Genre, opt => opt.MapFrom(src => Enum.Parse(typeof(Genre), src.Genre.ToString()) ))
                .ForMember(dest => dest.Authors, opt => opt.MapFrom(src => _authorService.GetAuthorsByIdList(src.Authors)))
                );
                var mapper = new Mapper(config);
                // Use mapper.
                Book book = mapper.Map<CreateBook, Book>(cBook);
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
