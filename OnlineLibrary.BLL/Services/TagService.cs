using OnlineLibrary.BLL.Interfaces;
using OnlineLibrary.Common.Entities;
using OnlineLibrary.Common.Exceptions;
using OnlineLibrary.Common.Helpers;
using OnlineLibrary.DAL.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace OnlineLibrary.BLL.Services
{
    public class TagService : ITagService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TagService(IUnitOfWork uow)
        {
            _unitOfWork = uow;
        }

        public int CreateTag(Tag tag)
        {
            ExceptionHelper.Check<OLBadRequest>(tag == null, "A null object came to the method");
            _unitOfWork.TagRepository.InsertTag(tag);
            _unitOfWork.Save();
            ExceptionHelper.Check<OLBadRequest>(tag.Id == 0, "The tag was not created");
            return tag.Id;
        }

        public List<Tag> GetAllTags()
        {
            return _unitOfWork.TagRepository.GetAllTags();
        }

        public List<Tag> GetTagsByIdList(List<int> tagsId)
        {
            List<Tag> tags = _unitOfWork.TagRepository.GetTagsByIdList(tagsId);
            ExceptionHelper.Check<OLNotFound>(tags == null || !tags.Any(), "Tags not found");
            return tags;
        }
    }
}
