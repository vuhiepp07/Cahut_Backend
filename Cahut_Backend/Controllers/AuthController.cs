using Cahut_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        private string CreateActiveMailBody(String UserId)
        {
            string bodyMsg = "";
            bodyMsg += "<h2>Chào mừng bạn đến với Cahut, nền tảng học online đa dạng và hiện đại" +
                ", bạn đã đăng kí tài khoản thành công, vui lòng click vào link sau để kích hoạt tài khoản</h2>";
            bodyMsg += $"<h3>https://localhost:44326/auth/activate/account/{UserId}</h3>";
            return bodyMsg;
        }

        [HttpPost("auth/register")]
        public ResponseMessage Register(RegisterModel obj)
        {
            if(provider.User.checkEmailExisted(obj.Email) == false)
            {
                int ret = provider.User.Register(obj);
                if (ret > 0)
                {
                    Guid UserId = provider.User.GetUserIdByUserEmail(obj.Email);
                    string bodyMsg = CreateActiveMailBody(UserId.ToString());
                    EmailMessage msg = new EmailMessage
                    {
                        EmailTo = obj.Email,
                        Subject = "Thư mời kích hoạt tài khoản",
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
                            message = "Đăng kí thành công, xin mời kiểm tra mail và làm theo hướng dẫn để kích hoạt tài khoản và sử dụng"
                        };
                    }
                    return new ResponseMessage
                    {
                        status = true,
                        data = null,
                        message = "Đăng kí thành công, gửi mail thất bại"
                    };
                }
                else
                {
                    return new ResponseMessage
                    {
                        status = true,
                        data = null,
                        message = "Đăng kí thất bại, xin mời thử lại với username khác"
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
            int ret = provider.User.ActivateAccount(Guid.Parse(UserId));
            if (ret > 0)
            {
                return new ResponseMessage
                {
                    status = true,
                    data = null,
                    message = "Kích hoạt tài khoản thành công, xin mời đăng nhập"
                };
            }
            else
            {
                return new ResponseMessage
                {
                    status = false,
                    data = null,    
                    message = "Kích hoạt tài khoản thất bại, xin mời thử lại"
                };
            }
        }

        [HttpPost("auth/login")]
        public ResponseMessage Login(LoginModel obj)
        {
            User usr = provider.User.Login(obj);
            if(usr != null)
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
                return new ResponseMessage
                {
                    status = true,
                    data = token,
                    message = "Đăng nhập thành công"
                };
            }
            else
            {
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Đăng nhập thất bại"
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
    }
}
