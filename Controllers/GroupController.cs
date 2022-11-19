using Cahut_Backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cahut_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : BaseController
    {
        public GroupController(SiteProvider provider) : base(provider)
        {
        }

        [HttpPost("group/join/{id}")]
        public int JoinGroup(string groupId)
        {

        }
    }
}
