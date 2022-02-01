using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using OnlineLibrary.API.Filters;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace OnlineLibrary.API.Controllers
{
    [Route("api/authors")]
    [ApiController]
    [TypeFilter(typeof(GenericExceptionFilter))]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorService _authorService;
        
        private readonly ILogger<AuthorController> _logger ;

        public AuthorController(IAuthorService iAuthor, ILogger<AuthorController> logger)
        {
            _authorService = iAuthor;
            _logger = logger;
        }

        // POST:  api/authors
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] Author author)
        {
            int id = await _authorService.CreateAuthorAsync(author);
            _logger.LogInformation($"New author created. Author ID = {id}.");
            return Ok(id);
        }

        // GET: api/authors
        [HttpGet]
        public async Task<IActionResult> GetAllAuthorsAsync()
        {
            List<Author> authors = await _authorService.GetAllAuthorsAsync();
            _logger.LogInformation($"Getting all authors. Authors count = {authors?.Count}.");
            return Ok(authors);
        }
    }
}
