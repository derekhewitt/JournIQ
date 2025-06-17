using JournalIQ.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JournalIQ.Data
{
    public class TagRepository : ITagRepository
    {
        private readonly DataContext _context;

        public TagRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<Tag> GetOrCreateAsync(string name)
        {
            var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == name);
            if (tag == null)
            {
                tag = new Tag { Name = name };
                _context.Tags.Add(tag);
                await _context.SaveChangesAsync();
            }
            return tag;
        }
    }

}
