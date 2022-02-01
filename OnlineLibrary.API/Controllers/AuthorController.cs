using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using OnlineLibrary.Common.Filters;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using log4net;

namespace OnlineLibrary.API.Controllers
{
    [Route("api/authors")]
    [ApiController]
    [TypeFilter(typeof(GenericExceptionFilter))]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorService _authorService;
        
        private static readonly ILog _logger = LogManager.GetLogger(typeof(AuthorController));
    
        public AuthorController(IAuthorService iAuthor)
        {
            _authorService = iAuthor;
        }

        // POST:  api/authors
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] Author author)
        {
            int id = await _authorService.CreateAuthorAsync(author);
            _logger.Info($"New author created. Author ID = {id}.");
            return Ok(id);
        }

        // GET: api/authors
        [HttpGet]
        public async Task<IActionResult> GetAllAuthorsAsync()
        {
            List<Author> authors = await _authorService.GetAllAuthorsAsync();
            _logger.Info($"Getting all authors. Authors count = {authors?.Count}.");
            return Ok(authors);
        }
    }
}
