using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnlineLibrary.BLL.Interfaces;
using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.API.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineLibrary.API.Controllers
{
    [Route("api/tags")]
    [ApiController]
    [TypeFilter(typeof(GenericExceptionFilter))]
    public class TagController : ControllerBase
    {
        private readonly ITagService _tagService;

        private readonly ILogger<TagController> _logger ;

        public TagController(ITagService iTag, ILogger<TagController> logger)
        {
            _tagService = iTag;
            _logger = logger;
        }

        // POST:  api/tags
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] Tag tag)
        {
            int id = await _tagService.CreateTagAsync(tag);
            _logger.LogInformation($"New tag created. Tag ID = {id}.");
            return Ok(id);
        }

        // GET: api/tags
        [HttpGet]
        public async Task<IActionResult> GetAllTagsAsync()
        {
            List<Tag> tags = await _tagService.GetAllTagsAsync();
            _logger.LogInformation($"Getting all tags. Tags count = {tags?.Count}.");
            return Ok(tags);
        }
    }
}
