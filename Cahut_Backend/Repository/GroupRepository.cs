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

        public int HandleGroupNumOfMembers(Guid GroupId, string type)
        {
            Group gr = GetGroupById(GroupId);
            if(type == "delete")
            {
                gr.NumOfMems = gr.NumOfMems - 1;
            }
            else
            {
                gr.NumOfMems = gr.NumOfMems + 1;
            }
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
                      orderby detail.RoleId ascending
                      where detail.GroupId == GroupId
                      select new
                      {
                          MemberName = usr.UserName,
                          Email = usr.Email,    
                          Role = detail.RoleId == 1 ? "Owner" : detail.RoleId == 2 ? "Co-owner" : "Member",
                          JoinedDate = detail.JoinedDate,
                      };
            return res;
        }

        public List<string> GetGrpEmails(Guid groupId)
        {
            var res = from detail in context.GroupDetail
                     join user in context.User
                     on detail.MemberId equals user.UserId
                     where detail.GroupId == groupId
                     select user.Email;
            return res.ToList();
        }

        public List<object> GetJoinedGroup(Guid userId)
        {
             var res = from detail in context.GroupDetail
                      join gr in context.Group
                      on detail.GroupId equals gr.GroupId
                      where detail.MemberId == userId && detail.RoleId != 1
                      select new
                      {
                          GroupName = gr.GroupName,
                          JoinedDate = detail.JoinedDate,
                          NumOfMems = gr.NumOfMems,
                          Role = detail.RoleId == 2 ? "Co-owner" : "Member",
                      };
            return res.ToList<object>();
        }

        public List<object> GetManagedGroup(Guid userId)
        {
            var res = from gr in context.Group
                      where gr.OwnerId == userId
                      select new
                      {
                          groupName = gr.GroupName,
                          numOfMems = gr.NumOfMems,
                          dateCreated = gr.DateCreated,
                          inviteLink = $"{Helper.TestingLink}/group/join/{gr.JoinGrString}"
                      };
            return res.ToList<object>();
        }

        public string GetMemberRoleInGroup(Guid MemberId, Guid GroupId)
        {
            var res = (from detail in context.GroupDetail
                       where detail.GroupId == GroupId && detail.MemberId == MemberId
                       select detail.RoleId).SingleOrDefault();
            var MemberRole = (from role in context.Role
                        where role.RoleId == res
                        select role.RoleName).SingleOrDefault();
            return MemberRole;
        }

        public bool CheckGroupNameExisted(string grName)
        {
            return context.Group.Any(p => p.GroupName == grName);
        }

        public bool AlreadyJoinedGroup(Guid UserId, Guid GroupId)
        {
            return context.GroupDetail.Any(p => p.MemberId == UserId && p.GroupId == GroupId);
        }

        public int DeleteGroup(Guid GroupId)
        {
            Group group = context.Group.Find(GroupId);
            context.Remove(group);
            return context.SaveChanges();
        }

        public string GetPresentationInGroup(Guid groupId)
        {
            Group group = context.Group.Find(groupId);
            return group.PresentationId;
        }

        public Guid GetGroupByPresentationId(string presentationId)
        {
            Group group = context.Group.Where(p => p.PresentationId == presentationId).FirstOrDefault();
            return group.GroupId;
        }

        public bool IsOwnerOrCoowner(Guid userId, Guid groupId)
        {
            return context.GroupDetail.Any(p => p.GroupId == groupId && p.MemberId == userId && (p.RoleId == 1 || p.RoleId == 2));
        }

        public bool isMember(Guid userId, Guid groupId)
        {
            return context.GroupDetail.Any(p => p.GroupId == groupId && p.MemberId == userId);
        }
    }
}
