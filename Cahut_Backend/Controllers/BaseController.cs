﻿using Cahut_Backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cahut_Backend.Controllers
{
    public class BaseController : ControllerBase, IDisposable
    {
        protected SiteProvider provider = new SiteProvider();
        public void Dispose()
        {
            provider.Dispose();
        }

    }
}
