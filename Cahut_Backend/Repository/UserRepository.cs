using Cahut_Backend.Models;
using System.Data;

namespace Cahut_Backend.Repository
{
    public class UserRepository : BaseRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {

        }
        public int UpdateResetPasswordStr(string email, string resetPasswordString)
        {
            User usr = context.User.Where(p => p.Email == email).SingleOrDefault();
            usr.ResetPasswordString = resetPasswordString;
            return context.SaveChanges();
        }

        public string GetUserEmailThroughResetPasswordCode(string ResetPasswordCode)
        {
            string email = context.User.Where(p => p.ResetPasswordString == ResetPasswordCode).Select(p => p.Email).SingleOrDefault();
            return email;
        }

        public Guid GetUserIdByUserName(string UserName)
        {
            var res = context.User.Where(p => p.UserName == UserName)
                                .Select(p => p.UserId)
                                .FirstOrDefault();
            if (res == null)
            {
                return Guid.Empty;
            }
            return res;
        }

        public Guid GetUserIdByUserEmail(string email)
        {
            var res = context.User.Where(p => p.Email == email)
                                .Select(p => p.UserId)
                                .FirstOrDefault();
            if (res == null)
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
                Password = Helper.Hash(obj.Email + "^@#%!@(!&^$" + obj.Password)
            };
            context.User.Add(temp);
            return context.SaveChanges();
        }

        public int ResetPassword(string email, string newPassword)
        {
            User temp = context.User.Where(p => p.Email == email).FirstOrDefault();
            temp.Password = Helper.Hash(email + "^@#%!@(!&^$" + newPassword);
            return context.SaveChanges();
        }

        public int ActivateAccount(Guid userId)
        {
            User res = context.User.SingleOrDefault(p => p.UserId == userId);
            if (res.AccountStatus == 1)
            {
                return -1;
            }
            res.AccountStatus = 1;
            return context.SaveChanges();
        }

        public object GetUserInfo(Guid userId)
        {
            User usr = context.User.Find(userId);
            if (usr != null)
            {
                return new
                {
                    UserName = usr.UserName,
                    Email = usr.Email,
                    Phone = usr.Phone,
                    AccountStatus = usr.AccountStatus
                };
            }
            return null;
        }

        public User GetUserById(Guid UserId)
        {
            User usr = context.User.Find(UserId);
            return usr;
        }

        public User Login(LoginModel obj)
        {
            var usr = from user in context.User
                      where user.Email == obj.Email && (user.Password.SequenceEqual(Helper.Hash(obj.Email + "^@#%!@(!&^$" + obj.Password)))
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

        public User LoginWithEmail(string email)
        {
            var usr = from user in context.User
                      where user.Email == email
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
            if (usr != null)
            {
                usr.RefreshToken = RefreshToken;
                usr.RefreshTokenExpiredTime = expiredTime;
                try
                {
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return 0;
        }

        public bool CheckUserExist(Guid UserId)
        {
            return context.User.Any(p => p.UserId == UserId);
        }

        public bool ValidatePassword(Guid userId, string password)
        {
            User usr = context.User.Find(userId);
            if (usr.Password.SequenceEqual(Helper.Hash(usr.Email + "^@#%!@(!&^$" + password)))
            {
                return true;
            }
            return false;
        }
        public int ChangePassword(Guid userId, string newPassword)
        {
            User usr = context.User.Find(userId);
            usr.Password = Helper.Hash(usr.Email + "^@#%!@(!&^$" + newPassword);
            return context.SaveChanges();
        }

        public bool CheckEmailExisted(string email)
        {
            bool existed = context.User.Any(p => p.Email == email);
            return existed;
        }

        public int UpdateUserInfo(Guid userId, UserInfoModel info)
        {
            User res = GetUserById(userId);
            res.UserName = info.UserName;
            res.Phone = info.Phone;
            return context.SaveChanges();
        }

        public int RegisterWithGoogleInfo(string email, string name, string avatar)
        {
            User usr = new User
            {
                Email = email,
                UserName = name,
                Avatar = avatar,
                Password = Helper.Hash(email + "^@#%!@(!&^$" + "gggggggg"),
                AccountStatus = 1
            };
            context.User.Add(usr);
            return context.SaveChanges();
        }

        public bool CheckResetCodeExisted(string code)
        {
            bool existed = context.User.Any(p => p.ResetPasswordString == code);
            return existed;
        }
    }
}
