using Cahut_Backend.Repository;

namespace Cahut_Backend.Models
{
    public class SiteProvider : BaseProvider
    {
        UserRepository product;
        public UserRepository Product
        {
            get
            {
                if (product == null)
                {
                    product = new UserRepository(Context);
                }
                return product;
            }
        }
    }
}
