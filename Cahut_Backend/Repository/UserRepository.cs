using System.Data;

namespace Cahut_Backend.Repository
{
    public class UserRepository : BaseRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }
    }
}
