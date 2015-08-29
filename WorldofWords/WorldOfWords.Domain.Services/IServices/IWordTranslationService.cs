using System.Collections.Generic;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.Domain.Services
{
    public interface IWordTranslationService
    {
        List<WordTranslation> GetTopBySearchWord(string searchWord, int languageId);
        List<WordTranslation> GetAllByTags(List<string> tags, int languageId);
        List<WordTranslation> GetByWordSuiteID(int id);
        int Exists(int originalWordId, int translationWordId);
        int Add(WordTranslation wordTranslation);
        List<WordTranslation> GetWordsFromInterval(int startOfInterval, int endOfInterval, int languageId);
        int GetAmountOfWordTranslationsByLanguage(int langID);
        WordTranslation GetWordTranslationById(int id);
        List<WordTranslation> GetWordsWithSearchValue(string searchValue, int startOfInterval, int endOfInterval, int languageId);
        int GetAmountOfWordsBySearchValues(string searchValue, int languageId);
    }
}