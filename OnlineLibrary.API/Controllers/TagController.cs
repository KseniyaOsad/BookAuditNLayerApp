using Microsoft.AspNetCore.Mvc;
using OnlineLibrary.BLL.Interfaces;
using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.Common.Filters;
using System.Threading.Tasks;

namespace OnlineLibrary.API.Controllers
{
    [Route("api/tags")]
    [ApiController]
    [TypeFilter(typeof(GenericExceptionFilter))]
    public class TagController : ControllerBase
    {

        private readonly ITagService _tagService;

        public TagController(ITagService iTag)
        {
            _tagService = iTag;
        }

        // POST:  api/tags
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] Tag tag)
        {
            return Ok(await _tagService.CreateTagAsync(tag));
        }

        // GET: api/tags
        [HttpGet]
        public async Task<IActionResult> GetAllTagsAsync()
        {
            return Ok(await _tagService.GetAllTagsAsync());
        }
    }
}
