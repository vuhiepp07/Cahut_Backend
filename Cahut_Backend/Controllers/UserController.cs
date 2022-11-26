using Cahut_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cahut_Backend.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        [HttpGet("user/info"),Authorize]
        public ResponseMessage GetSelfInfo()
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return new ResponseMessage
            {
                status = true,
                data = provider.User.GetUserInfo(userId),
                message = "Get user info success"
            };
        }

        [HttpPost("user/info/update"), Authorize]
        public ResponseMessage UpdateInfo(UserInfoModel info)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            int ret = provider.User.UpdateUserInfo(userId, info);
            return new ResponseMessage
            {
                status = ret == -1?false:true,
                data = null,
                message = ret == -1?"Update user info failed, email already exist": "Update user info success"
            };
        }
    }
}
