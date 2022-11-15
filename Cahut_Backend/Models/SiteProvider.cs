using Cahut_Backend.Repository;

namespace Cahut_Backend.Models
{
    public class SiteProvider : BaseProvider
    {
        public SiteProvider(IConfiguration configuration, AppDbContext context) : base(configuration, context)
        {
        }
    }
}
