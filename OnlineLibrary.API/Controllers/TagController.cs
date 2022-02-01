using log4net;
using Microsoft.AspNetCore.Mvc;
using OnlineLibrary.BLL.Interfaces;
using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.Common.Filters;
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

        private static readonly ILog _logger = LogManager.GetLogger(typeof(TagController));

        public TagController(ITagService iTag)
        {
            _tagService = iTag;
        }

        // POST:  api/tags
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] Tag tag)
        {
            int id = await _tagService.CreateTagAsync(tag);
            _logger.Info($"New tag created. Tag ID = {id}.");
            return Ok(id);
        }

        // GET: api/tags
        [HttpGet]
        public async Task<IActionResult> GetAllTagsAsync()
        {
            List<Tag> tags = await _tagService.GetAllTagsAsync();
            _logger.Info($"Getting all tags. Tags count = {tags?.Count}.");
            return Ok(tags);
        }
    }
}
