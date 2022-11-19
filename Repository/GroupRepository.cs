using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace Cahut_Backend.Repository
{
    public class GroupRepository : BaseRepository
    {
        public GroupRepository(IDbConnection connection, AppDbContext dbContext) : base(connection, dbContext)
        {
        }

        public List<User> GetMembers()
        {

        }

        public User GetGrCoOwner()
        {

        }

        pubic User GetGrOwner()
        {

        }

        public int 

        public int CreateInviteLink()
        {

        }
        [Authorize(Roles ="admin")]
        public int DeleteInviteLink()
        {

        }

        public int GetInviteLink()
        {

        }
    }
}
