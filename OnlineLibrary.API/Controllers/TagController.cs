using Microsoft.AspNetCore.Mvc;
using OnlineLibrary.BLL.Interfaces;
using OnlineLibrary.Common.Entities;
using OnlineLibrary.Common.Filters;

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
        public IActionResult Create([FromBody] Tag tag)
        {
            return Ok(_tagService.CreateTag(tag));
        }

        // GET: api/tags
        [HttpGet]
        public IActionResult GetAllTags()
        {
            return Ok(_tagService.GetAllTags());
        }
    }
}
