using OnlineLibrary.Common.DBEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineLibrary.BLL.Interfaces
{
    public interface ITagService
    {
        Task<List<Tag>> GetAllTagsAsync();

        Task<int> CreateTagAsync(Tag tag);

        Task<List<Tag>> GetTagsByIdListAsync(List<int> tagsId);
    }
}
