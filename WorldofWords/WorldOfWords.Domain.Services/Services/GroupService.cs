using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using WorldOfWords.API.Models;
using WorldOfWords.Domain.Models;
using WorldOfWords.Infrastructure.Data.EF.Factory;

namespace WorldOfWords.Domain.Services
{
    public class GroupService : IGroupService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public GroupService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public List<Group> GetAll(int userId)
        {
            using(var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                return uow.GroupRepository.GetAll().Where(g => g.OwnerId == userId).Include(group => group.Course).ToList();
            }
        }

        public bool ContainsGroupName(GroupModel groupModel)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                var group = uow.GroupRepository.GetAll().FirstOrDefault(g => g.Name == groupModel.Name);
                return group != null;
            }
        }

        public void Add(Group newGroup)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                uow.GroupRepository.Add(newGroup);
                uow.Save();
            }
        }

        public Group GetById(int groupId, int userId)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                return uow.GroupRepository.GetAll().Where(g => g.OwnerId == userId).Include(g => g.Course).FirstOrDefault(g => g.Id == groupId);
            }
        }

        public void DeleteById(int groupId)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                uow.EnrollmentRepository.Delete(uow.EnrollmentRepository.GetAll().Where(e => e.GroupId == groupId));
                uow.GroupRepository.Delete(groupId);
                uow.Save();
            }
        }
    }
}