using Cahut_Backend.Models;
using System.Data;

namespace Cahut_Backend.Repository
{
    public abstract class BaseRepository
    {
        protected IDbConnection connection;
        protected AppDbContext dbContext;
        public BaseRepository(IDbConnection connection, AppDbContext dbContext)
        {
            this.connection = connection;
            this.dbContext = dbContext;
        }
    }
}
