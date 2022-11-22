using Cahut_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cahut_Backend.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        [HttpGet("user/selfinfo/{email}"),Authorize]
        public ResponseMessage GetSelfInfo(string email)
        {
            return new ResponseMessage
            {
                status = true,
                data = provider.User.GetUserInfo(email),
                message = "Lấy thông tin người dùng thành công"
            };
        }
    }
}
