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
    public class HeadingSlideController : BaseController
    {
        [HttpPost("/slide/heading/editSlide"), Authorize]
        public ResponseMessage editHeadingSlide(object editSlideModel)
        {
            JObject objTemp = JObject.Parse(editSlideModel.ToString());

            string presentationId = (string)objTemp["presentationId"];
            string slideId = (string)objTemp["slideId"];
            string headingContent = (string)objTemp["headingContent"];
            string subHeadingContent = (string)objTemp["subHeadingContent"];

            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (!provider.HeadingSlide.CheckSlideIdExisted(slideId))
            {
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Slide does not existed"
                };
            }

            bool isCollab = provider.Presentation.isCollaborator(Guid.Parse(presentationId), userId);
            bool isExisted = provider.Presentation.presentationExisted(Guid.Parse(presentationId), userId);
            if (isExisted || isCollab)
            {
                int updateResult = provider.HeadingSlide.EditHeadingSlide(slideId, headingContent, subHeadingContent);
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

        [HttpGet("/slide/heading/getSlide")]
        public ResponseMessage GetHeadingSlide(string slideId)
        {
            if (provider.HeadingSlide.CheckSlideIdExisted(slideId))
            {
                return new ResponseMessage
                {
                    status = true,
                    data = provider.HeadingSlide.GetHeadingSlide(slideId),
                    message = "Get heading slide successfully"
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
