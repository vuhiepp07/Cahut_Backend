
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
                usr.Token.RefreshToken = String.Empty;
                usr.Token.AccessToken = String.Empty;
            }
            return context.SaveChanges();
        }
    }
}
