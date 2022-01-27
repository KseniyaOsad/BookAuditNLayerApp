using OnlineLibrary.BLL.Interfaces;
using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.Common.Exceptions;
using OnlineLibrary.Common.Extensions;
using OnlineLibrary.DAL.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLibrary.BLL.Services
{
    public class TagService : ITagService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TagService(IUnitOfWork uow)
        {
            _unitOfWork = uow;
        }

        public async Task<int> CreateTagAsync(Tag tag)
        {
            ExceptionExtensions.Check<OLBadRequest>(tag == null, "A null object came to the method");
            _unitOfWork.TagRepository.InsertTag(tag);
            await _unitOfWork.SaveAsync();
            ExceptionExtensions.Check<OLBadRequest>(tag.Id == 0, "The tag was not created");
            return tag.Id;
        }

        public Task<List<Tag>> GetAllTagsAsync()
        {
            return _unitOfWork.TagRepository.GetAllTagsAsync();
        }

        public async Task<List<Tag>> GetTagsByIdListAsync(List<int> tagsId)
        {
            List<Tag> tags = await _unitOfWork.TagRepository.GetTagsByIdListAsync(tagsId);
            ExceptionExtensions.Check<OLNotFound>(tags == null || !tags.Any(), "Tags not found");
            return tags;
        }
    }
}
