using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using OnlineLibrary.Common.Filters;
using System.Threading.Tasks;

namespace OnlineLibrary.API.Controllers
{
    [Route("api/authors")]
    [ApiController]
    [TypeFilter(typeof(GenericExceptionFilter))]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        public AuthorController(IAuthorService iAuthor)
        {
            _authorService = iAuthor;
        }

        // POST:  api/authors
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] Author author)
        {
            return Ok(await _authorService.CreateAuthorAsync(author));
        }

        // GET: api/authors
        [HttpGet]
        public async Task<IActionResult> GetAllAuthorsAsync()
        {
            return Ok(await _authorService.GetAllAuthorsAsync());
        }
    }
}
