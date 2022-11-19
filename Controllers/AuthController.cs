using Cahut_Backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cahut_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        public AuthController(SiteProvider provider) : base(provider)
        {
        }

        //[HttpPost("auth/login")]
        //public User Login(AuthModel obj)
        //{
        //    User temp = provider.Login(obj);
        //    if(temp != null)
        //    {
        //        return temp;
        //    }
        //}

        [HttpPost("auth/logout/{id}")]
        public int Logout(string UserId)
        {
            return provider.User.Logout(Guid.Parse(UserId));
        }

        [HttpGet("auth/activateaccount/{UserId}")]
        public int ActivateAccount(string UserId)
        {
            return provider.User.ActivateAccount(Guid.Parse(UserId));
        }
    }
}
