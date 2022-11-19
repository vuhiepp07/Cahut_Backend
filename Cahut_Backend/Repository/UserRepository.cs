using Cahut_Backend.Models;
using System.Data;

namespace Cahut_Backend.Repository
{
    public class UserRepository : BaseRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {

        }

        public Guid GetUserId(string UserName)
        {
            var res = context.User.Where(p => p.UserName == UserName)
                                .Select(p => p.UserId)
                                .FirstOrDefault();
            if(res == null)
            {
                return Guid.Empty;
            }
            return res;
        }

        public int Register(RegisterModel obj)
        {
            User temp = new User
            {
                UserName = obj.UserName,
                Email = obj.Email,
                Password = Helper.Hash(obj.UserName + "^@#%!@(!&^$" + obj.Password)
            };
            context.User.Add(temp);
            return context.SaveChanges();
        }

        public int ActivateAccount(Guid userId)
        {
            User res = context.User.SingleOrDefault(p => p.UserId == userId);
            if(res.AccountStatus == 1)
            {
                return -1;
            }
            res.AccountStatus = 1;
            return context.SaveChanges();
        }

        public User Login(LoginModel obj)
        {
            User user = context.User.SingleOrDefault(p => p.UserName == obj.UserName && p.Password == Helper.Hash(obj.UserName + "^@#%!@(!&^$" + obj.Password));
            return user;
        }

        public int ClearToken(Guid userId)
        {
            User res = context.User.SingleOrDefault(p => p.UserId == userId);
            if(res != null)
            {
                res.Token.RefreshToken = String.Empty;
                res.Token.AccessToken = String.Empty;
                return context.SaveChanges();
            }
            return 0;
        }
    }
}
