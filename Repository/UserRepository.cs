using Cahut_Backend.Models;
using System.Data;

namespace Cahut_Backend.Repository
{
    public class UserRepository : BaseRepository
    {
        public UserRepository(IDbConnection connection, AppDbContext dbContext) : base(connection, dbContext)
        {
        }

        public int Login(AuthModel obj)
        {

        }

        public int Logout(Guid userId)
        {
            var usr = dbContext.User.Where<User>(p => p.UserId == userId).FirstOrDefault();
            usr.Token.AccessToken = null;
            usr.Token.RefreshToken = null;
            return dbContext.SaveChanges();
        }

        public int Register(User obj)
        {
            obj.Password = Helper.Hash(obj.UserName + "^@#%!@(!&^$" + obj.Password).ToString();
            dbContext.Add(obj);
            return dbContext.SaveChanges();
        }

        public int ActivateAccount(Guid userId)
        {
            var usr = dbContext.User.Where<User>(p => p.UserId == userId).FirstOrDefault();
            if( usr!= null)
            {
                usr.AccountStatus = 1;
            }
            return dbContext.SaveChanges();
        }

        public User GetUserInfo(Guid userId)
        {
            var usr = dbContext.User.Where(p => p.UserId == userId)
                        .Select(p => new User
                        {
                            UserId = p.UserId,
                            UserName = p.UserName,
                            Email = p.Email,
                            Phone = p.Phone,
                            AccountStatus = p.AccountStatus
                        })
                        .FirstOrDefault();
            return usr;
        }

        public User EditInfo(User obj)
        {
            var usr = from user in dbContext.User
                      where user.UserId == obj.UserId
                      select user;
            return usr.SingleOrDefault();
        }

    }
}
