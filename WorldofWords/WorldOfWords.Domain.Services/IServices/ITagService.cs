using System.Collections.Generic;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.Domain.Services
{
    public interface ITagService
    {
        int Exists(string name);
        int Add(Tag tag);
    }
}
