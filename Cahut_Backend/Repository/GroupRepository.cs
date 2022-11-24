using Cahut_Backend.Models;
using System.Data;

namespace Cahut_Backend.Repository
{
    public class GroupRepository : BaseRepository
    {
        public GroupRepository(AppDbContext context) : base(context)
        {
        }

        public Group GetGroupWithInviteString(string inviteString)
        {
            return context.Group.Where(p => p.JoinGrString.Equals(inviteString)).SingleOrDefault();
        }

        public int AddMember(Guid GroupId, Guid UserId)
        {
            GroupDetail detail = new GroupDetail
            {
                GroupId = GroupId,
                MemberId = UserId,
                RoleId = 3,
                JoinedDate = DateTime.UtcNow.AddHours(7)
            };
            context.GroupDetail.Add(detail);
            return context.SaveChanges();
        }

        public int CreateGroup(Guid UserId, string grName)
        {
            Guid grId = Guid.NewGuid();
            Group group = new Group
            {
                GroupId = grId,
                DateCreated = DateTime.UtcNow.AddHours(7),
                OwnerId = UserId,
                GroupName = grName,
                JoinGrString = CreateGroupInviteString()
            };
            context.Group.Add(group);
            if(context.SaveChanges() > 0)
            {
                GroupDetail grDetail = new GroupDetail
                {
                    GroupId = grId,
                    MemberId = UserId,
                    RoleId = 1,
                    JoinedDate = DateTime.UtcNow.AddHours(7),
                };
                context.GroupDetail.Add(grDetail);
                return context.SaveChanges();
            }
            return 0;
        }

        public Group GetGroupByName(string grName)
        {
            return context.Group.Where(p => p.GroupName == grName).SingleOrDefault();
        }

        public Group GetGroupById(Guid GroupId)
        {
            return context.Group.Find(GroupId);
        }

        public int DeleteInviteLink(Guid GroupId)
        {
            Group gr = GetGroupById(GroupId);
            gr.JoinGrString = null;
            return context.SaveChanges();
        }

        public string CreateGroupInviteString()
        {
            string rand = Helper.RandomString(16);
            return rand;
        }

    }
}
