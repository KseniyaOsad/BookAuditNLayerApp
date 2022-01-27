using OnlineLibrary.Common.DBEntities;
using System.Collections.Generic;

namespace OnlineLibrary.DAL.Interfaces
{
    public interface ITagRepository
    {
        List<Tag> GetAllTags();

        void InsertTag(Tag tag);

        List<Tag> GetTagsByIdList(List<int> tagsId);
    }
}
