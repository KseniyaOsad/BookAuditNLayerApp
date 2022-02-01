using Microsoft.AspNetCore.Mvc;
using System;
using OnlineLibrary.BLL.Interfaces;
using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.API.Model;
using AutoMapper;
using System.Collections.Generic;
using Microsoft.AspNetCore.JsonPatch;
using OnlineLibrary.Common.Filters;
using OnlineLibrary.Common.DBEntities.Enums;
using OnlineLibrary.Common.EntityProcessing;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OnlineLibrary.Common.EntityProcessing.Pagination;
using log4net;

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

        private static readonly ILog _logger = LogManager.GetLogger(typeof(BookController));

        public BookController(IBookService iBook, IAuthorService iAuthor, ITagService iTag, IMapper mapper)
        {
            _bookService = iBook;
            _authorService = iAuthor;
            _tagService = iTag;
            _mapper = mapper;
        }

        // Post: api/books/search
        [HttpPost("search")]
        public async Task<IActionResult> FilterBookAsync([FromBody] BookProcessing bookProcessing)
        {
            PaginatedList<Book> paginatedList = await _bookService.FilterBooksAsync(bookProcessing);
            _logger.Info($"Filter books. Books count = {paginatedList?.TotalCount}.");
            return Ok(paginatedList);
        }

        // GET: api/books/[id]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookByIdAsync(int? id)
        {
            Book book = await _bookService.GetBookByIdAsync(id);
            _logger.Info($"Getting book by id. Book's id = {book?.Id}");
            return Ok(book);
        }

        // POST:  api/books/
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateBook cBook)
        {
            List<Tag> tags = await _tagService.GetTagsByIdListAsync(cBook.Tags);
            _logger.Info($"Getting tags by id list. Tags count = {tags?.Count}");

            List<Author> authors = await _authorService.GetAuthorsByIdListAsync(cBook.Authors);
            _logger.Info($"Getting authors by id list. Authors count = {authors?.Count}");

            Book book = _mapper.Map<CreateBook, Book>(cBook);
            book.Authors = authors;
            book.Tags = tags;
            _logger.Info("Map createBook to book.");

            int id = await _bookService.CreateBookAsync(book);
            _logger.Info($"New book created. Book ID = {authors?.Count}");
            return Ok(id);
        }

        // Patch:  api/books/[id]
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdatePatchAsync(int Id, [FromBody] JsonPatchDocument<Book> book)
        {
            await _bookService.UpdatePatchAsync(Id, book);
            _logger.Info($"Update book. Book ID = {Id}");
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
