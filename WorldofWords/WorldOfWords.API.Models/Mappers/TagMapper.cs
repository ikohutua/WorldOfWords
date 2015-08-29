using WorldOfWords.API.Models;
using WorldOfWords.Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace WorldOfWords.API.Models
{
    public class TagMapper : ITagMapper
    {
        public Tag Map(TagModel tag)
        {
            return new Tag()
            {   
                Id = tag.Id,
                Name = tag.Name
            };
        }

        public TagModel Map(Tag tag)
        {
            return new TagModel()
            {
                Id = tag.Id,
                Name = tag.Name
            };
        }

        public List<Tag> MapRange(List<TagModel> tags)
        {
            return tags.Select(t => Map(t)).ToList();
        }
    }
}
