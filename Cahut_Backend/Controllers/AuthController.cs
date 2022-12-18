using Cahut_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Cahut_Backend.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private IConfiguration _configuration;
        public AuthController(IConfiguration configuration) : base()
        {
            this._configuration = configuration;
        }

        [HttpGet("/auth/version")]
        public ResponseMessage GetVersion()
        {
            return new ResponseMessage
            {
                status = true,
                data = DateTime.UtcNow.AddHours(7),
                message = "Ver 2"
            };
        }

        private string CreateActiveMailBody(String UserId)
        {
            string bodyMsg = "";
            bodyMsg += "<h2>Welcome to Cahut, a diverse and modern online learning platform" +
                ", You have successfully registered an account, please click on the following link to activate your account</h2>";

            //Uncomment this when deploy or test with Front-end in the local host
            bodyMsg += $"<h3>{Helper.TestingLink}/account/activate/{UserId}</h3>";

            //Uncomment this when test apis with postman only
            //bodyMsg += $"<h3>{Helper.TestingLink}/auth/activate/account/{UserId}</h3>";
            return bodyMsg;
        }

        [HttpPost("auth/register")]
        public ResponseMessage Register(RegisterModel obj)
        {
            if(provider.User.CheckEmailExisted(obj.Email) == false)
            {
                int ret = provider.User.Register(obj);
                if (ret > 0)
                {
                    Guid UserId = provider.User.GetUserIdByUserEmail(obj.Email);
                    string bodyMsg = CreateActiveMailBody(UserId.ToString());
                    EmailMessage msg = new EmailMessage
                    {
                        EmailTo = obj.Email,
                        Subject = "Account activation mail",
                        Content = bodyMsg
                    };
                    EmailSender sender = provider.Email.GetMailSender();
                    string sendMailResult = Helper.SendEmails(sender, msg, _configuration);
                    if (sendMailResult == "Send mail success")
                    {
                        provider.Email.increaseMailSent(sender.usr);
                        return new ResponseMessage
                        {
                            status = true,
                            data = null,
                            message = "Register success, please check activation mail in your email and follow the instruction"
                        };
                    }
                    return new ResponseMessage
                    {
                        status = true,
                        data = null,
                        message = "Register success but send activation mail failed"
                    };
                }
                else
                {
                    return new ResponseMessage
                    {
                        status = true,
                        data = null,
                        message = "Register failed, please try with another email"
                    };
                }
            }
            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Failed to register, email already existed"
            };
            
        }

        [HttpGet("auth/activate/account/{UserId}")]
        public ResponseMessage ActivateAccount(string UserId)
        {
            if(provider.User.CheckUserExist(Guid.Parse(UserId)) == false)
            {
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Account activate failed, this activation link is invalid"
                };
            }
            int ret = provider.User.ActivateAccount(Guid.Parse(UserId));
            if (ret > 0)
            {
                return new ResponseMessage
                {
                    status = true,
                    data = null,
                    message = "Account activated, please login to proceed"
                };
            }
            else if(ret == -1)
            {
                return new ResponseMessage
                {
                    status = false,
                    data = null,    
                    message = "Account already activated, please login"
                };
            }
            else
            {
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Account activate failed, please login to proceed"
                };
            }
        }
        private Token SaveUserInfoAndCreateTokens(User usr)
        {
            ClaimsIdentity claims = new ClaimsIdentity(new Claim[]
               {
                    new Claim(ClaimTypes.Name, usr.UserName),
                    new Claim(ClaimTypes.Email, usr.Email),
                    new Claim(ClaimTypes.NameIdentifier, usr.UserId.ToString()),
               });
            Token token = new Token
            {
                AccessToken = TokenServices.CreateToken(claims),
                RefreshToken = TokenServices.CreateRefreshToken()
            };
            int saveToDbResult = provider.User.UpdateUserTokens(usr.UserId, token.RefreshToken, DateTime.UtcNow.AddDays(7));
            return token;
        }

        [HttpPost("auth/googlelogin")]
        public ResponseMessage GoogleLogin(GoogleLoginModel model)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(model.GoogleCredential);
            string email = jwtSecurityToken.Claims.First(claim => claim.Type == "email").Value;
            string name = jwtSecurityToken.Claims.First(claim => claim.Type == "name").Value;
            string avatar = jwtSecurityToken.Claims.First(claim => claim.Type == "picture").Value;
            if (provider.User.CheckEmailExisted(email) == true)
            {
                User usr = provider.User.LoginWithEmail(email);
                if(usr.UserName != name || usr.Avatar != avatar)
                {
                    provider.User.UpdateUserInfo(usr.UserId, new UserInfoModel { UserName = name });
                }
                Token token = SaveUserInfoAndCreateTokens(usr);
                return new ResponseMessage
                {
                    status = true,
                    data = token,
                    message = "Login success"
                };
            }
            else
            {
                int registerResult = provider.User.RegisterWithGoogleInfo(email, name, avatar);
                if(registerResult > 0)
                {
                    User usr = provider.User.LoginWithEmail(email);
                    Token token = SaveUserInfoAndCreateTokens(usr);
                    return new ResponseMessage
                    {
                        status = true,
                        data = token,
                        message = "Login success"
                    };
                }
                else
                {
                    return new ResponseMessage
                    {
                        status = false,
                        data = null,
                        message = "Login failed"
                    };
                }
            }
        }

        [HttpPost("auth/login")]
        public ResponseMessage Login(LoginModel obj)
        {
            User usr = provider.User.Login(obj);
            if(usr != null)
            {
                if(usr.AccountStatus != 1)
                {
                    return new ResponseMessage
                    {
                        status = false,
                        data = null,
                        message = "Login failed, your account is not activated, please check your mail and follow the activation link to login and proceed"
                    };
                }
                Token token = SaveUserInfoAndCreateTokens(usr);
                return new ResponseMessage
                {
                    status = true,
                    data = token,
                    message = "Login success"
                };
            }
            else
            {
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Login failed"
                };
            }
        }

        [HttpPost("auth/changepassword"), Authorize]
        public ResponseMessage ChangePassword(ChangePasswordModel obj)
        {
            Guid UserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            bool validatePassword = provider.User.ValidatePassword(UserId, obj.CurrentPassword);
            if(validatePassword == true)
            {
                int result = provider.User.ChangePassword(UserId, obj.NewPassword);
                return new ResponseMessage
                {
                    status = result > 0 ? true : false,
                    data = null,
                    message = result > 0 ? "Change user password success" : "Change user password failed"
                };
            }
            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Change password failed"
            };
        }

        [HttpGet("auth/logout"), Authorize]
        public void Logout()
        {
            Guid UserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            provider.Token.ClearUsrToken(UserId);
        }

        [HttpGet("auth/forgotpassword")]
        public ResponseMessage ForgotPassword(string email)
        {
            bool emailExisted = provider.User.CheckEmailExisted(email);
            if (emailExisted)
            {
                string newPassword = Helper.RandomString(8);
                int resetPasswordResult = provider.User.ResetPassword(email, newPassword);
                if(resetPasswordResult > 0)
                {
                    string bodyMsg = "<h2>Hello user, you just required to reset password in our system, hear is your new password: " + $"{newPassword}" +
                ", please login and change your password once again for security purpose</h2>";
                    EmailMessage msg = new EmailMessage
                    {
                        EmailTo = email,
                        Subject = "Account reset password mail",
                        Content = bodyMsg
                    };
                    EmailSender sender = provider.Email.GetMailSender();
                    string sendMailResult = Helper.SendEmails(sender, msg, _configuration);
                    if (sendMailResult == "Send mail success")
                    {
                        provider.Email.increaseMailSent(sender.usr);
                        return new ResponseMessage
                        {
                            status = true,
                            data = null,
                            message = "Reset password success, your new password has been sent to your registered email."
                        };
                    }
                }
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Reset password failed, please try again"
                };
            }
            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Reset password failed, your email is not exist in ours system"
            };
        }
    }
}
