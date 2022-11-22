
using Cahut_Backend.Models;
using System.Data;

namespace Cahut_Backend.Repository
{
    public class TokenRepository : BaseRepository
    {
        public TokenRepository(AppDbContext context) : base(context)
        {
        }
        
        public int ClearUsrToken(Guid UserId)
        {
            User usr = context.User.Where<User>(p => p.UserId == UserId).SingleOrDefault();
            if(usr != null)
            {
                usr.RefreshToken = String.Empty;
                usr.RefreshTokenExpiredTime = DateTime.UtcNow;
            }
            return context.SaveChanges();
        }

        public bool ValidateRefreshToken(Guid UserId, string RefreshToken)
        {
            User usr = context.User.Where(p => p.UserId == UserId).SingleOrDefault();
            if(usr.RefreshToken == RefreshToken)
            {
                return true;
            }
            return false;
        }
    }
}
