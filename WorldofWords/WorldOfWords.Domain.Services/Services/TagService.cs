using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using WorldOfWords.Domain.Models;
using WorldOfWords.Infrastructure.Data.EF;

namespace WorldOfWords.Domain.Services
{
    public class TagService : ITagService
    {
        public int Exists(string name)
        {
            using (var context = new WorldOfWordsDatabaseContext())
            {
                var tag = context.Tags.FirstOrDefault(t => t.Name == name);

                return tag != null ? tag.Id : 0;
            }
        }

        public int Add(Tag tag)
        {
            using (var context = new WorldOfWordsDatabaseContext())
            {
                context.Tags.Add(tag);
                context.SaveChanges();
                return tag.Id;
            }
        }

        
    }
}
