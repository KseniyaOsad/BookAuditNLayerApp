using Microsoft.AspNetCore.Mvc;
using System;
using OnlineLibrary.BLL.Interfaces;
using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.Common.Extensions;
using OnlineLibrary.API.Model;
using AutoMapper;
using System.Collections.Generic;
using OnlineLibrary.Common.Exceptions;
using Microsoft.AspNetCore.JsonPatch;
using OnlineLibrary.Common.Filters;
using OnlineLibrary.Common.EntityProcessing.Pagination;
using OnlineLibrary.Common.DBEntities.Enums;
using OnlineLibrary.Common.EntityProcessing.Filtration;
using OnlineLibrary.Common.EntityProcessing;

namespace OnlineLibrary.API.Controllers
{
    [Route("api/books")]
    [ApiController]
    [TypeFilter(typeof(GenericExceptionFilter))]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        private readonly IAuthorService _authorService;

        private readonly ITagService _tagService;

        private readonly IMapper _mapper;

        public BookController(IBookService iBook, IAuthorService iAuthor, ITagService iTag, IMapper mapper)
        {
            _bookService = iBook;
            _authorService = iAuthor;
            _tagService = iTag;
            _mapper = mapper;
        }

        // Post: api/Book/
        [HttpPost]
        public IActionResult GetAllBooks([FromBody]PaginationOptions paginationOptions)
        {
            return Ok(_bookService.GetAllBooks(paginationOptions));
        }

        // Post: api/Book/Filtration
        [HttpPost("search")]
        public IActionResult FilterBook([FromBody] BookProcessing bookProcessing)
        {
            return Ok(_bookService.FilterBooks(bookProcessing));
        }

        // GET: api/Books/[id]
        [HttpGet("{id}")]
        public IActionResult GetBookById(int? id)
        {

            return Ok(_bookService.GetBookById(id));
        }

        // POST:  api/Books/
        [HttpPost]
        public IActionResult Create([FromBody] CreateBook cBook)
        {
            List<Tag> tags = _tagService.GetTagsByIdList(cBook.Tags);
            List<Author> authors = _authorService.GetAuthorsByIdList(cBook.Authors);

            Book book = _mapper.Map<CreateBook, Book>(cBook);
            book.Authors = authors;
            book.Tags = tags;
            return Ok(_bookService.CreateBook(book));
        }

        [HttpPatch("{id}")]
        public IActionResult UpdatePatch(int Id, [FromBody] JsonPatchDocument<Book> book)
        {
            _bookService.UpdatePatch(Id, book);
            return Ok(_bookService.GetBookById(Id));
        }


        // GET: api/Book/GetAllGenres
        [HttpGet("genres")]
        public IActionResult GetAllGenres()
        {
            return Ok(Enum.GetNames(typeof(Genre)));
        }
    }
}
