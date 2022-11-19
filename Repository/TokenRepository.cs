
using System.Data;

namespace Cahut_Backend.Repository
{
    public class TokenRepository : BaseRepository
    {
        public TokenRepository(IDbConnection connection, AppDbContext dbContext) : base(connection, dbContext)
        {
        }

        public Token Create(Guid UserId)
        {

        }

        public int ClearTokens(Guid UserId)
        {

        }

        public string RefreshAccessToken()
        {

        }

        public string RefreshRefreshToken()
        {

        }
    }
}
