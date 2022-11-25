using Cahut_Backend.Models;
using System.Data;

namespace Cahut_Backend.Repository
{
    public class UserRepository : BaseRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {

        }

        public Guid GetUserIdByUserName(string UserName)
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

        public object GetUserInfo(string email)
        {
            return context.User.Select(p => new
            {
                UserName = p.UserName,
                Email = p.Email,
                Phone = p.Phone,
                AccountStatus = p.AccountStatus
            }).SingleOrDefault(p => p.Email == email);
        }

        public User GetUserById(Guid UserId)
        {
            User usr = context.User.Find(UserId);
            return usr;
        }

        public User Login(LoginModel obj)
        {   
            var usr = from user in context.User
                      where user.UserName == obj.UserName && (user.Password.SequenceEqual(Helper.Hash(obj.UserName + "^@#%!@(!&^$" + obj.Password)))
                      select new User
                      {
                          UserId = user.UserId,
                          Avatar = user.Avatar,
                          UserName = user.UserName,
                          Email = user.Email,
                          Phone = user.Phone,
                          AccountStatus = user.AccountStatus
                      };
            return usr.SingleOrDefault();
        }

        public int UpdateUserTokens(Guid UserId, string RefreshToken, DateTime expiredTime)
        {
            User usr = GetUserById(UserId);
            if(usr != null)
            {
                usr.RefreshToken = RefreshToken;
                usr.RefreshTokenExpiredTime = expiredTime;
                try
                {
                    context.SaveChanges();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return 0;
        }

        public bool ValidatePassword(Guid userId, string password)
        {
            User usr = context.User.Find(userId);
            if(usr.Password.SequenceEqual(Helper.Hash(usr.UserName + "^@#%!@(!&^$" + password)))
            {
                return true;
            }
            return false;
        }
        public int ChangePassword(Guid userId, string newPassword)
        {
            User usr = context.User.Find(userId);
            usr.Password = Helper.Hash(usr.UserName + "^@#%!@(!&^$" + newPassword);
            return context.SaveChanges();
        }
    }
}
