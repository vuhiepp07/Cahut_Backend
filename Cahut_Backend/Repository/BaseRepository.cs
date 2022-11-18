using Cahut_Backend.Models;
using System.Data;

namespace Cahut_Backend.Repository
{
    public abstract class BaseRepository
    {
        protected AppDbContext context;
        public BaseRepository(AppDbContext context)
        {
            this.context = context;
        }
    }
}
