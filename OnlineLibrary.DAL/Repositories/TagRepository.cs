using Microsoft.EntityFrameworkCore;
using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.DAL.EF;
using OnlineLibrary.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLibrary.DAL.Repositories
{
    public class TagRepository : ITagRepository
    {
        BookContext _context;

        public TagRepository(BookContext context)
        {
            _context = context;
        }

        public Task<List<Tag>> GetAllTagsAsync()
        {
            return _context.Tag
              .Include(t => t.Books)
              .OrderBy(t => t.Name).ToListAsync();
        }

        public Task<List<Tag>> GetTagsByIdListAsync(List<int> tagsId)
        {
            return  _context.Tag.Where(t => tagsId.Contains(t.Id)).ToListAsync();
        }

        public void InsertTag(Tag tag)
        {
            _context.Add(tag);
        }
    }
}
