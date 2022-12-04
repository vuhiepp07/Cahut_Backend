using Cahut_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Security.Claims;

namespace Cahut_Backend.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class PresentationController : BaseController
    {
        [HttpPost("/presentation/create"), Authorize]
        public ResponseMessage Create(string presentationName)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            bool isExisted = provider.Presentation.CheckExisted(userId, presentationName);
            if(isExisted == false)
            {
                int createResult = provider.Presentation.Create(userId, presentationName);
                return new ResponseMessage
                {
                    status = createResult > 0 ? true : false,
                    data = null,
                    message = createResult > 0 ? "Create presentation success" : "Create presentation failed, please try again"
                };
            }
            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Create failed, presentation is already exist"
            };
        }

        [HttpPost("/presentation/update"), Authorize]
        public ResponseMessage Update(object PresentInfo)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            JObject objTemp = JObject.Parse(PresentInfo.ToString());
            string oldName = (string)objTemp["oldName"];
            string newName = (string)objTemp["newName"];
            Presentation presentation = provider.Presentation.GetPresentationByNameAndTeacherId(oldName, userId);
            if(presentation is not null)
            {
                bool newNameExisted = provider.Presentation.CheckExisted(userId, newName);
                if(newNameExisted == true)
                {
                    return new ResponseMessage
                    {
                        status =false,
                        data = null,
                        message = $"Presentation '{newName}' already existed in your presentation list"
                    };
                }
                int updateResult = provider.Presentation.Update(presentation.PresentationId, newName);
                return new ResponseMessage
                {
                    status = updateResult > 0 ? true : false,
                    data = null,
                    message = updateResult > 0 ? "Rename presentation success" : "Rename presentation failed, please try again"
                };
            }
            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Update failed, presentation does not exist"
            };
        }

        [HttpPost("/presentation/delete"), Authorize]
        public ResponseMessage Delete(string presentationName)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            Presentation presentation = provider.Presentation.GetPresentationByNameAndTeacherId(presentationName, userId);
            if (presentation is not null)
            {
                int deleteResult = provider.Presentation.Delete(presentation.PresentationId);
                return new ResponseMessage
                {
                    status = deleteResult > 0 ? true : false,
                    data = null,
                    message = deleteResult > 0 ? "Delete presentation success" : "Delete presentation failed, please try again"
                };
            }
            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Delete failed, presentation does not exist"
            };
        }

        [HttpGet("/presentation/getlist"), Authorize]
        public ResponseMessage GetPresentationList()
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            List<object> list = provider.Presentation.GetPresentationList(userId);
            return new ResponseMessage
            {
                status = true,
                data = list,
                message = list.Count > 0 ? "Get presentation list success" : "Your presentation list is empty, please create a new presentation first"
            };
        }
    }
}
