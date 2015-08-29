using System.Collections.Generic;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.Domain.Services
{
    public interface IWordSuiteService
    {
        List<WordSuite> GetTeacherWordSuites(int id);
        List<WordSuite> GetWordSuitesByOwnerAndLanguageId(int userId, int languageId);
        WordSuite GetByID(int id);
        WordSuite GetWithNotStudiedWords(int id);
        void SetTime(int id);
        void CopyWordsuitesForUsersByGroup(List<User> users, int groupId);
        void RemoveWordSuitesForEnrollment(int enrollmentId);
        double GetWordSuiteProgress(int id);
        int Add(WordSuite wordSuite);
        bool Update(WordSuite wordSuite);
        bool Delete(int id);
    }
}
