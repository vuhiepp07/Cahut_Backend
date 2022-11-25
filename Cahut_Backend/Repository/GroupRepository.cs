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

        public int SetMemberRole(Guid UserId, string grName, string roleName)
        {
            Group gr = GetGroupByName(grName);
            if (gr == null)
            {
                return -1;
            }
            else
            {
                var detail = from grp in context.Group
                             join grdetail in context.GroupDetail
                             on grp.GroupId equals grdetail.GroupId
                             where grp.GroupName == grName && grdetail.MemberId == UserId
                             select grdetail;
                int roleId = (from role in context.Role
                             where role.RoleName.ToLower().Equals(roleName.ToLower())
                             select role.RoleId).FirstOrDefault<int>();
                detail.SingleOrDefault<GroupDetail>().RoleId = roleId;
                return context.SaveChanges();
            }
        }

        public int DeleteMember(Guid groupId, Guid MemberId)
        {
            GroupDetail memberInGroup = context.GroupDetail.Where(p => p.GroupId == groupId && p.MemberId == MemberId).FirstOrDefault();
            if(memberInGroup != null)
            {
                context.GroupDetail.Remove(memberInGroup);
                return context.SaveChanges();
            }
            return 0;
        }

        public object getAllGrMembers(Guid GroupId)
        {
            var res = from detail in context.GroupDetail
                      join usr in context.User
                      on detail.MemberId equals usr.UserId
                      where detail.GroupId == GroupId
                      select new
                      {
                          MemberName = usr.UserName,
                          Role = detail.RoleId == 1 ? "Owner" : detail.RoleId == 2 ? "Co-owner" : "Member",
                          JoinedDate = detail.JoinedDate,
                      };
            return res;
        }
    }
}
