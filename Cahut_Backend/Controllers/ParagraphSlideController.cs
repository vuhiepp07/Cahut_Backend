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
    public class ParagraphSlideController : BaseController
    {
        [HttpPost("/slide/paragraph/editSlide"), Authorize]
        public ResponseMessage UpdateParagraphSlide(object updateSlideModel)
        {
            JObject objTemp = JObject.Parse(updateSlideModel.ToString());

            string presentationId = (string)objTemp["presentationId"];
            string slideId = (string)objTemp["slideId"];
            string headingContent = (string)objTemp["headingContent"];
            string paragraphContent = (string)objTemp["paragraphContent"];

            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (!provider.ParagraphSlide.CheckSlideIdExisted(slideId))
            {
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Edit failed, slide does not existed"
                };
            }

            bool isCollab = provider.Presentation.isCollaborator(Guid.Parse(presentationId), userId);
            bool isExisted = provider.Presentation.presentationExisted(Guid.Parse(presentationId), userId);
            if (isExisted || isCollab)
            {
                int updateResult = provider.ParagraphSlide.UpdateParagraphSlide(slideId, headingContent, paragraphContent);
                return new ResponseMessage
                {
                    status = updateResult >= 0 ? true : false,
                    data = null,
                    message = updateResult >= 0 ? "Update paragraph slide successfully" : "Failed to update slide"
                };
            }
            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Edit failed, Only owner and collaborators can edit new slide"
            };
        }


        [HttpGet("/slide/paragraph/getSlide")]
        public ResponseMessage GetParagraphSlide(string slideId)
        {
            if (provider.ParagraphSlide.CheckSlideIdExisted(slideId))
            {
                return new ResponseMessage
                {
                    status = true,
                    data = provider.ParagraphSlide.GetParagraphSlide(slideId),
                    message = "Get paragraph slide successfully"
                };
            }
            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Slide does not existed"
            };
           
        }
    }

   
}
