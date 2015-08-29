using System.Collections.Generic;

namespace WorldOfWords.API.Models
{
    public class SearchWordTranslationModel
    {
        public string SearchWord {get; set;}
        public List<string> Tags { get; set; }
        public int LanguageId { get; set; }
        public bool SearchByTag { get; set; }
    }
}
