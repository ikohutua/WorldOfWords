using System.Collections.Generic;
using System.Linq;
using WorldOfWords.Domain.Models;
using WorldOfWords.Infrastructure.Data.EF.Factory;

namespace WorldOfWords.Domain.Services.Services
{
    public class WordService : IWordService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public WordService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public List<Word> GetTopBySearchWord(string searchWord, int languageId, int count)
        {
            List<Word> words;
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                words = uow.WordRepository.GetAll()
                    .Where(w => w.Value.Contains(searchWord) &&
                                w.LanguageId == languageId)
                    .OrderBy(w => w.Value.IndexOf(searchWord))
                    .ThenBy(w => w.Value)
                    .Take(count)
                    .ToList();
            }
            //using (var context = new WorldOfWordsDatabaseContext())
            //{
            //    words = context.Words
            //        .Where(w => w.Value.Contains(searchWord) &&
            //                    w.LanguageId == languageId)
            //        .OrderBy(w => w.Value.IndexOf(searchWord))
            //        .ThenBy(w => w.Value)
            //        .Take(count)
            //        .ToList();
            //}
            return words;
        }

        public List<Word> GetAllWords()
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                return uow.WordRepository.GetAll().ToList();
            }
            //using (var context = new WorldOfWordsDatabaseContext())
            //{
            //    return context.Words.ToList();
            //}
        }

        public List<Word> GetAllWordsBySpecificLanguage(int languageId)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                return (from words in uow.WordRepository.GetAll() where words.LanguageId == languageId select words).ToList();
            }
            //using (var context = new WorldOfWordsDatabaseContext())
            //{
            //    return (from words in context.Words where words.LanguageId == languageId select words).ToList();
            //}

        }

        public int Exists(string value, int languageId)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                var word = uow.WordRepository.GetAll().FirstOrDefault(w => w.Value == value && w.LanguageId == languageId);

                if (word != null)
                {
                    return word.Id;
                }
                else
                {
                    return 0;
                }
            }
            //using (var context = new WorldOfWordsDatabaseContext())
            //{
            //    var word = context.Words.FirstOrDefault(w => w.Value == value && w.LanguageId == languageId);

            //    if (word != null)
            //    {
            //        return word.Id;
            //    }
            //    else
            //    {
            //        return 0;
            //    }
            //}
        }

        public int Add(Word word)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                if (Exists(word.Value, word.LanguageId) > 0)
                {
                    return -1;
                }
                uow.WordRepository.Add(word);
                uow.Save();
                return word.Id;
            }
            //using (var context = new WorldOfWordsDatabaseContext())
            //{
            //    if (Exists(word.Value, word.LanguageId) > 0)
            //    {
            //        return -1;
            //    }
            //    context.Words.Add(word);
            //    context.SaveChanges();
            //    return word.Id;
            //}
        }


        public Word GetWordById(int id)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                return uow.WordRepository.GetById(id);
            }
            //using(var context = new WorldOfWordsDatabaseContext())
            //{
            //    return context.Words.Where(item => item.Id == id).FirstOrDefault();
            //}
        }
    }
}