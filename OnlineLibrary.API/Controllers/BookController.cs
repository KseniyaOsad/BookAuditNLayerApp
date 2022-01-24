using Microsoft.AspNetCore.Mvc;
using System;
using OnlineLibrary.BLL.Interfaces;
using OnlineLibrary.Common.Entities;
using OnlineLibrary.Common.Helpers;
using OnlineLibrary.API.Model;
using AutoMapper;
using System.Collections.Generic;
using OnlineLibrary.Common.Exceptions;
using OnlineLibrary.Common.Exceptions.Enum;
using Microsoft.AspNetCore.JsonPatch;
using OnlineLibrary.Common.Filters;
using OnlineLibrary.Common.Pagination;

namespace OnlineLibrary.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [TypeFilter(typeof(GenericExceptionFilter))]
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

        // GET: api/Book/GetAllBooks/
        [HttpGet("{PageNumber}/{PageSize}")]
        public IActionResult GetAllBooks(int? PageNumber, int? PageSize)
        {
            PageNumber = PageNumber ?? 1;
            PageSize = PageSize ?? 1;
            return Ok(_bookService.GetAllBooks(new PaginationOptions((int)PageNumber, (int)PageSize)));
        }

        // GET: api/Book/GetAllBooks
        [HttpGet]
        public IActionResult GetAllBooks()
        {
            return Ok(_bookService.GetAllBooks());
        }

        // GET: api/Book/GetBooksWithFilters
        [HttpGet("{authorId}/{name}/{reservation}/{inArchieve}")]
        public IActionResult GetBooksWithFilters(int? authorId, string name, int? reservation, int? inArchieve)
        {
            return Ok(_bookService.FilterBooks(authorId, name, reservation, inArchieve));
        }

        // GET: api/Book/GetBookById/[id]
        [HttpGet("{id}")]
        public IActionResult GetBookById(int? id)
        {
            return Ok(_bookService.GetBookById(id));
        }

        // POST:  api/Book/Create
        [HttpPost]
        public IActionResult Create([FromBody] CreateBook cBook)
        {
            // необходимо настроить добавление с тегами!
            List<Author> authors = _authorService.GetAuthorsByIdList(cBook.Authors);

            Book book = _mapper.Map<CreateBook, Book>(cBook);
            book.Authors = authors;
            return Ok(_bookService.CreateBook(book));
        }

        // PUT:  api/Book/UpdateReservation/[id]
        [HttpPut("{id}")]
        public IActionResult UpdateReservation(int Id, [FromBody] Book book)
        {
            ExceptionHelper.Check<OLBadRequest>(Id != book.Id, "IDs don't match");
            _bookService.ChangeBookReservation(Id, book.Reserve);
            Book b = _bookService.GetBookById(Id);
            return Ok(b);
        }

        // PUT:  api/Book/UpdateArchievation/[id]
        [HttpPut("{id}")]
        public IActionResult UpdateArchievation(int Id, [FromBody] Book book)
        {
            ExceptionHelper.Check<OLBadRequest>(Id != book.Id, "IDs don't match");
            _bookService.ChangeBookArchievation(Id, book.InArchive);
            Book b = _bookService.GetBookById(Id);
            return Ok(b);
        }

        [HttpPatch("{id}")]
        public IActionResult UpdatePatch(int Id, [FromBody] JsonPatchDocument<Book> book)
        {
            _bookService.UpdatePatch(Id, book);
            return Ok(_bookService.GetBookById(Id));
        }


        // GET: api/Book/GetAllGenres
        [HttpGet]
        public IActionResult GetAllGenres()
        {
            return Ok(Enum.GetNames(typeof(Genre)));
        }
    }
}
