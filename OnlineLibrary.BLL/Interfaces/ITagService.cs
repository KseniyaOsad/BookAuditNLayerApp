using OnlineLibrary.Common.DBEntities;
using System.Collections.Generic;

namespace OnlineLibrary.BLL.Interfaces
{
    public interface ITagService
    {
        List<Tag> GetAllTags();

        int CreateTag(Tag tag);

        List<Tag> GetTagsByIdList(List<int> tagsId);
    }
}
