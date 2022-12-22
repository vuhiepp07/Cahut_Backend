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
    public class SlideCommonController : BaseController
    {
        //Xử lí hàm chung cho các slide trong controller này (Create, update, delete slide..)
        //Các controller con như MultipleChoiceSlideController hay ParagraphController để xử lí 1 số field riêng mà chỉ loại slide đó mới có
        [HttpPost("/slide/create"), Authorize]
        public ResponseMessage CreateSlide(object createSlideModel)
        {
            
            JObject objTemp = JObject.Parse(createSlideModel.ToString());
            
            string presentationId = (string)objTemp["presentationId"];
            string slideType = (string)objTemp["slideType"];

            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

          
            bool isCollab = provider.Presentation.isCollaborator(Guid.Parse(presentationId), userId);
            bool isExisted = provider.Presentation.presentationExisted(Guid.Parse(presentationId), userId);
            if (isExisted || isCollab)
            {
                int addResult = 0;
                
                switch (slideType)
                {
                    case "heading":
                        string headingSlideId = Helper.RandomString(8);
                        while (provider.HeadingSlide.CheckSlideIdExisted(headingSlideId) == true)
                        {
                            headingSlideId = Helper.RandomString(8);
                        }
                        addResult = provider.HeadingSlide.CreateHeadingSlide(Guid.Parse(presentationId), headingSlideId);
                        break;
                    case "paragraph":
                        string paraSlideId = Helper.RandomString(8);
                        while (provider.ParagraphSlide.CheckSlideIdExisted(paraSlideId) == true)
                        {
                            paraSlideId = Helper.RandomString(8);
                        }
                        addResult = provider.ParagraphSlide.CreateParagraphSlide(Guid.Parse(presentationId), paraSlideId);
                        break;
                    case "multipleChoice":
                        string multipleSlideId = Helper.RandomString(8);
                        while (provider.MultipleChoiceSlide.CheckSlideIdExisted(multipleSlideId) == true)
                        {
                            multipleSlideId = Helper.RandomString(8);
                        }
                        addResult = provider.MultipleChoiceSlide.CreateMultipleChoiceSlide(Guid.Parse(presentationId), multipleSlideId);
                        break;
                    default:
                        return new ResponseMessage
                        {
                            status = false,
                            data = null,
                            message = "Invalid slide type"
                        };
                }
                return new ResponseMessage
                {
                    status = addResult > 0 ? true : false,
                    data = null,
                    message = addResult > 0 ? "Create slide successfully" : "Failed to create slide"
                };
            }
            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Create failed, Only owner and collaborators can add new slide"
            };
        }

        [HttpPost("/slide/delete"), Authorize]
        public ResponseMessage DeleteSlide(object deleteSlideModel)
        {
            JObject objTemp = JObject.Parse(deleteSlideModel.ToString());

            string slideId = (string)objTemp["slideId"];
            string slideType = (string)objTemp["slideType"];
            string presentationId = (string)objTemp["presentationId"];

            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            bool isCollab = provider.Presentation.isCollaborator(Guid.Parse(presentationId), userId);
            bool isExisted = provider.Presentation.presentationExisted(Guid.Parse(presentationId), userId);

            if (isExisted || isCollab)
            {
                int deleteResult = 0;
                switch (slideType)
                {
                    case "heading":
                        if (!provider.HeadingSlide.CheckSlideIdExisted(slideId))
                        {
                            return new ResponseMessage
                            {
                                status = false,
                                data = null,
                                message = "slide does not existed"
                            };
                        }
                        deleteResult = provider.HeadingSlide.DeleteHeadingSlide(slideId);
                        
                        break;
                    case "paragraph":
                        if (!provider.ParagraphSlide.CheckSlideIdExisted(slideId))
                        {
                            return new ResponseMessage
                            {
                                status = false,
                                data = null,
                                message = "slide does not existed"
                            };
                        }
                        deleteResult = provider.ParagraphSlide.DeleteParagraphSlide(slideId);
                        
                        break;
                    case "multipleChoice":
                        if (!provider.MultipleChoiceSlide.CheckSlideIdExisted(slideId))
                        {
                            return new ResponseMessage
                            {
                                status = false,
                                data = null,
                                message = "slide does not existed"
                            };
                        }
                        deleteResult = provider.MultipleChoiceSlide.Delete(slideId);
                        break;
                    default:
                        return new ResponseMessage
                        {
                            status = false,
                            data = null,
                            message = "Invalid slide type"
                        };
                }
                return new ResponseMessage
                {
                    status = deleteResult > 0 ? true : false,
                    data = null,
                    message = deleteResult > 0 ? "Delete slide successfully" : "Failed to delete slide"
                };
            }
            return new ResponseMessage
            {
                status = false,  
                data = null,
                message = "Delete failed, Only owner and collaborators can delete slide"
            };
        }
    }
}
