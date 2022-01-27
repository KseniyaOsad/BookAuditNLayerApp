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
using System.Threading.Tasks;

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

        // Post: api/books/
        [HttpPost]
        public async Task<IActionResult> GetAllBooksAsync([FromBody]PaginationOptions paginationOptions)
        {
            return Ok(await _bookService.GetAllBooksAsync(paginationOptions));
        }

        // Post: api/books/search
        [HttpPost("search")]
        public async Task<IActionResult> FilterBookAsync([FromBody] BookProcessing bookProcessing)
        {
            return Ok(await _bookService.FilterBooksAsync(bookProcessing));
        }

        // GET: api/books/[id]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookByIdAsync(int? id)
        {
            return Ok(await _bookService.GetBookByIdAsync(id));
        }

        // POST:  api/books/
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateBook cBook)
        {
            List<Tag> tags = await _tagService.GetTagsByIdListAsync(cBook.Tags);
            List<Author> authors = await _authorService.GetAuthorsByIdListAsync(cBook.Authors);

            Book book = _mapper.Map<CreateBook, Book>(cBook);
            book.Authors = authors;
            book.Tags = tags;
            return Ok(await _bookService.CreateBookAsync(book));
        }

        // Patch:  api/books/[id]
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdatePatchAsync(int Id, [FromBody] JsonPatchDocument<Book> book)
        {
            await _bookService.UpdatePatchAsync(Id, book);
            return Ok(await _bookService.GetBookByIdAsync(Id));
        }

        // GET: api/books/genres
        [HttpGet("genres")]
        public IActionResult GetAllGenres()
        {
            return Ok(Enum.GetNames(typeof(Genre)));
        }
    }
}
