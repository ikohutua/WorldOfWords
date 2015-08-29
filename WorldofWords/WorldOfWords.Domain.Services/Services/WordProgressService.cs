using System;
using System.Collections.Generic;
using System.Linq;
using WorldOfWords.Domain.Models;
using WorldOfWords.Infrastructure.Data.EF.Factory;

namespace WorldOfWords.Domain.Services
{
    public class WordProgressService : IWordProgressService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public WordProgressService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public bool IsStudentWord(WordProgress wordProgress)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                return uow
                        .WordProgressRepository.GetAll()
                        .Single(wp => wp.WordSuiteId == wordProgress.WordSuiteId &&
                                      wp.WordTranslationId == wordProgress.WordTranslationId).IsStudentWord;
            }
            //using (var context = new WorldOfWordsDatabaseContext())
            //{
            //    return context
            //            .WordProgresses
            //            .Single(wp => wp.WordSuiteId == wordProgress.WordSuiteId &&
            //                          wp.WordTranslationId == wordProgress.WordTranslationId).IsStudentWord;
            //}
        }

        public void CopyProgressesForUsersInGroup(List<User> users, int groupId)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                var group = uow.GroupRepository.GetById(groupId);
                if (group == null)
                {
                    throw new ArgumentException("Group with id you are requesting does not exist");
                }
                var originalWordsuites = group.Course.WordSuites.Where(w => w.PrototypeId == null);
                var progressesToAdd = new List<WordProgress>();
                foreach (var user in users)
                {
                    foreach (var wordsuite in originalWordsuites)
                    {
                        var copiedWordSuite = group.Course.WordSuites.FirstOrDefault(w => w.PrototypeId == wordsuite.Id && w.OwnerId == user.Id);
                        progressesToAdd.AddRange(copiedWordSuite.PrototypeWordSuite.WordProgresses.Select(wp => new WordProgress
                        {
                            WordSuiteId = copiedWordSuite.Id,
                            WordTranslationId = wp.WordTranslationId,
                            Progress = 0
                        }));
                    }
                }
                uow.WordProgressRepository.Add(progressesToAdd);
                uow.Save();
            }
            //using(var context = new WorldOfWordsDatabaseContext())
            //{
            //    var group = context.Groups.FirstOrDefault(g => g.Id == groupId);
            //    if(group == null)
            //    {
            //        throw new ArgumentException("Group with id you are requesting does not exist");
            //    }
            //    var originalWordsuites = group.Course.WordSuites.Where(w => w.PrototypeId == null);
            //    var progressesToAdd = new List<WordProgress>();
            //    foreach(var user in users)
            //    {
            //        foreach(var wordsuite in originalWordsuites)
            //        {
            //            var copiedWordSuite = group.Course.WordSuites.FirstOrDefault(w => w.PrototypeId == wordsuite.Id && w.OwnerId == user.Id);
            //            progressesToAdd.AddRange(copiedWordSuite.PrototypeWordSuite.WordProgresses.Select(wp => new WordProgress
            //                {
            //                    WordSuiteId = copiedWordSuite.Id,
            //                    WordTranslationId = wp.WordTranslationId,
            //                    Progress = 0
            //                }));
            //        }
            //    }
            //    context.WordProgresses.AddRange(progressesToAdd);
            //    context.SaveChanges();
            //}
        }

        public bool AddRange(List<WordProgress> wordProgressRange)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                foreach (var wordProgress in wordProgressRange)
                {
                    uow.WordProgressRepository.AddOrUpdate(wordProgress);
                }
                uow.Save();
                return true;
            }
            //using (var context = new WorldOfWordsDatabaseContext())
            //{
            //    var wordProgresses = context.WordProgresses;
            //    foreach (var wordProgress in wordProgressRange)
            //    {
            //        wordProgresses.AddOrUpdate(wordProgress);
            //    }
            //    context.SaveChanges();
            //    return true;
            //}
        }

        public bool RemoveRange(List<WordProgress> wordProgressRange)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                wordProgressRange.ForEach(wp =>
                    uow
                    .WordProgressRepository
                    .Delete(uow
                            .WordProgressRepository.GetAll()
                            .Single(dbWp => dbWp.WordSuiteId == wp.WordSuiteId
                                            && dbWp.WordTranslationId == wp.WordTranslationId)));
                uow.Save();
            }
            //using (var context = new WorldOfWordsDatabaseContext())
            //{
            //    wordProgressRange.ForEach(wp =>
            //        context
            //        .WordProgresses
            //        .Remove(context
            //                .WordProgresses
            //                .Single(dbWp => dbWp.WordSuiteId == wp.WordSuiteId
            //                                && dbWp.WordTranslationId == wp.WordTranslationId)));
            //    context.SaveChanges();
            //}
            return true;
        }

        public bool RemoveByStudent(WordProgress wordProgress)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                if (IsStudentWord(wordProgress))
                {
                    uow
                    .WordProgressRepository
                    .Delete(uow
                        .WordProgressRepository.GetAll()
                        .Single(wp => wp.WordSuiteId == wordProgress.WordSuiteId &&
                                      wp.WordTranslationId == wordProgress.WordTranslationId));
                    uow.Save();
                    return true;
                }
                return false;
            }
            //using (var context = new WorldOfWordsDatabaseContext())
            //{
            //    if (IsStudentWord(wordProgress))
            //    {
            //        context
            //        .WordProgresses
            //        .Remove(context
            //            .WordProgresses
            //            .Single(wp => wp.WordSuiteId == wordProgress.WordSuiteId &&
            //                          wp.WordTranslationId == wordProgress.WordTranslationId));
            //        context.SaveChanges();
            //        return true;
            //    }
            //    return false;
            //}
        }

        public bool IncrementProgress(int wordSuiteId, int wordTranslationId)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                var wordProgress = uow.WordProgressRepository.GetAll().First(x => (x.WordSuiteId == wordSuiteId
                            && x.WordTranslationId == wordTranslationId));
                ++(wordProgress.Progress);
                uow.WordProgressRepository.AddOrUpdate(wordProgress);
                uow.Save();
                return true;
            }
            //using (var context = new WorldOfWordsDatabaseContext())
            //{
            //    var wordProgress = context.WordProgresses.First(x => (x.WordSuiteId == wordSuiteId
            //                && x.WordTranslationId == wordTranslationId));
            //    ++(wordProgress.Progress);
            //    context.WordProgresses.AddOrUpdate(wordProgress);
            //    context.SaveChanges();
            //    return true;
            //}
        }

        public void RemoveProgressesForEnrollment(int enrollmentId)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                Enrollment enrollment = uow.EnrollmentRepository.GetById(enrollmentId);
                if (enrollment == null)
                {
                    throw new ArgumentException("Enrollment with id you are requesting does not exist");
                }
                List<int> originalWordsuitesIds = enrollment.Group.Course.WordSuites.Where(w => w.PrototypeId == null).Select(w => w.Id).ToList();
                var wordsuitesToDeleteFrom = enrollment.Group.Course.WordSuites
                    .Where(w => w.OwnerId == enrollment.User.Id && w.PrototypeId != null
                        && originalWordsuitesIds.Contains((int)w.PrototypeId)).ToList();
                uow.WordProgressRepository.Delete(wordsuitesToDeleteFrom.SelectMany(ws => ws.WordProgresses));
                uow.Save();
            }
            //using(var context = new WorldOfWordsDatabaseContext())
            //{
            //    Enrollment enrollment = context.Enrollments.FirstOrDefault(e => e.Id == enrollmentId);
            //    if (enrollment == null)
            //    {
            //        throw new ArgumentException("Enrollment with id you are requesting does not exist");
            //    }
            //    List<int> originalWordsuitesIds = enrollment.Group.Course.WordSuites.Where(w => w.PrototypeId == null).Select(w => w.Id).ToList();
            //    var wordsuitesToDeleteFrom = enrollment.Group.Course.WordSuites
            //        .Where(w => w.OwnerId == enrollment.User.Id && w.PrototypeId != null 
            //            && originalWordsuitesIds.Contains((int)w.PrototypeId)).ToList();
            //    context.WordProgresses.RemoveRange(wordsuitesToDeleteFrom.SelectMany(ws => ws.WordProgresses));
            //    context.SaveChanges();
            //}
        }
    }
}
