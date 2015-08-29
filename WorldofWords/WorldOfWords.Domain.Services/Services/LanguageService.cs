using System.Collections.Generic;
using System.Linq;
using WorldOfWords.Domain.Models;
using WorldOfWords.Domain.Services.IServices;
using WorldOfWords.Infrastructure.Data.EF.Factory;

namespace WorldOfWords.Domain.Services.Services
{
    /// <summary>
    /// Responsible for obtaining and manipulating the list of languages.
    /// </summary>
    public class LanguageService : ILanguageService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public LanguageService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        /// <summary>
        /// Adds a new language to the database.
        /// </summary>
        /// <param name="language">The language to be added to the database.</param>
        /// <returns>The id of a new database record, or -1, if such language already exists.</returns>
        public int AddLanguage(Language language)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                if (uow.LanguageRepository.GetAll()
                    .Any(l => l.Name == language.Name))
                {
                    return -1;
                }
                uow.LanguageRepository.Add(language);
                uow.Save();
                return language.Id;
            }
            //using (var context = new WorldOfWordsDatabaseContext())
            //{
            //    if (context
            //        .Languages
            //        .Any(l => l.Name == language.Name))
            //    {
            //        return -1;
            //    }
            //    context.Languages.Add(language);
            //    context.SaveChanges();
            //    return language.Id;
            //}
        }

        /// <summary>
        /// Removes language from the database.
        /// </summary>
        /// <param name="id"> ID of the language to be removed from the database.</param>
        /// <returns>True, if language successfully deleted, false, if no language with such ID</returns>
        public bool RemoveLanguage(int id)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                var language = uow.LanguageRepository.GetById(id);

                if (language != null)
                {
                    if (language.Courses.Count == 0)
                    {
                        if (language.WordSuites.Count != 0)
                        {
                            var wordSuites = uow.WordSuiteRepository.GetAll().Where(ws => ws.LanguageId == id);
                            var wordProgresses = uow.WordProgressRepository.GetAll().Where(wp => wordSuites.Select(ws => ws.Id).Contains(wp.WordSuiteId));
                            uow.WordProgressRepository.Delete(wordProgresses);
                            uow.WordSuiteRepository.Delete(wordSuites);
                        }
                        if (language.Words.Count != 0)
                        {
                            var wordTranslations = uow.WordTranslationRepository.GetAll().Where(wt => wt.OriginalWord.LanguageId == id);
                            uow.WordTranslationRepository.Delete(wordTranslations);

                            var words = uow.WordRepository.GetAll().Where(w => w.LanguageId == id);
                            uow.WordRepository.Delete(words);
                        }

                        uow.LanguageRepository.Delete(language);
                        uow.Save();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            //using (var context = new WorldOfWordsDatabaseContext())
            //{
            //    var language = context.Languages.SingleOrDefault(l => l.Id == id);

            //    if (language != null)
            //    {
            //        if (language.Courses.Count == 0) 
            //        {
            //            if (language.WordSuites.Count != 0)
            //            {
            //                var wordSuites = context.WordSuites.Where(ws => ws.LanguageId == id);
            //                var wordProgresses = context.WordProgresses.Where(wp => wordSuites.Select(ws => ws.Id).Contains(wp.WordSuiteId));
            //                context.WordProgresses.RemoveRange(wordProgresses);
            //                context.WordSuites.RemoveRange(wordSuites);
            //            }
            //            if (language.Words.Count != 0)
            //            {
            //                var wordTranslations = context.WordTranslations.Where(wt => wt.OriginalWord.LanguageId == id);
            //                context.WordTranslations.RemoveRange(wordTranslations);

            //                var words = context.Words.Where(w => w.LanguageId == id);
            //                context.Words.RemoveRange(words);
            //            }

            //            context.Languages.Remove(language);
            //            context.SaveChanges();
            //            return true;
            //        }
            //        else
            //        {
            //            return false;
            //        }
            //    }
            //    else
            //    {
            //        return false;
            //    }
            //}
        }

        /// <summary>
        /// Returns a list of all languages in the database.
        /// </summary>
        /// <returns></returns>
        public List<Language> GetAllLanguages()
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                return uow.LanguageRepository.GetAll().ToList();
            }
            //using (var context = new WorldOfWordsDatabaseContext())
            //{
            //    return context.Languages.ToList();
            //}
        }
    }
}