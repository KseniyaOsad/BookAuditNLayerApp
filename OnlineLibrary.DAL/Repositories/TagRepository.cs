using Microsoft.EntityFrameworkCore;
using OnlineLibrary.Common.Entities;
using OnlineLibrary.DAL.EF;
using OnlineLibrary.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnlineLibrary.DAL.Repositories
{
    public class TagRepository : ITagRepository
    {
        BookContext _context;

        public TagRepository(BookContext context)
        {
            _context = context;
        }

        public List<Tag> GetAllTags()
        {
            return _context.Tag
              .Include(t => t.Books)
              .OrderBy(t => t.Name).ToList();
        }

        public List<Tag> GetTagsByIdList(List<int> tagsId)
        {
            return _context.Tag.Where(t => tagsId.Contains(t.Id)).ToList();
        }

        public void InsertTag(Tag tag)
        {
            _context.Add(tag);
        }
    }
}
