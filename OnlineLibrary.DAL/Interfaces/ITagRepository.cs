using OnlineLibrary.Common.DBEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineLibrary.DAL.Interfaces
{
    public interface ITagRepository
    {
        Task<List<Tag>> GetAllTagsAsync();

        Task CreateTagAsync(Tag tag);

        Task<List<Tag>> GetTagsByIdListAsync(List<int> tagsId);
    }
}
