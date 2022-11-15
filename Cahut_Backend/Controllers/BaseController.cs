using Cahut_Backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cahut_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected SiteProvider provider;
        public BaseController(SiteProvider provider)
        {
            this.provider = provider;
        }

    }
}
