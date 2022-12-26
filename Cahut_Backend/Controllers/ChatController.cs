using Cahut_Backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Cahut_Backend.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class ChatController : BaseController
    {
        [HttpPost("/chat/sendMessage")]
        public ResponseMessage SendMessage(object messageModel)
        {
            JObject objTemp = JObject.Parse(messageModel.ToString());
            string presentationId = (string)objTemp["presentationId"];
            string message = (string)objTemp["message"];
            string senderEmail = (string)objTemp["senderEmail"];

            if (provider.Presentation.GetPresentationName(Guid.Parse(presentationId)) == null)
            {
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Presentation does not existed",
                };
            }

            if (!provider.Presentation.isPresentating(Guid.Parse(presentationId)))
            {
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Presentation has not been presented yet"
                };
            }

            int sendResult = provider.Chat.AddMessage(Guid.Parse(presentationId), message, senderEmail);
            if (sendResult == -1)
            {
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Email does not existed",
                };
            }
            return new ResponseMessage
            {
                status = sendResult > 0 ? true : false,
                data = null,
                message = sendResult > 0 ? "Send message successfully" : "Failed to send message"
            };
        }

        [HttpGet("/chat/getMessage")]
        public ResponseMessage GetMessages(string presentationId)
        {
            if (!provider.Presentation.isPresentating(Guid.Parse(presentationId)))
            {
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Presentation has not been presented yet"
                };
            }
            if (provider.Presentation.GetPresentationName(Guid.Parse(presentationId)) == null)
            {
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Presentation does not existed",
                };
            }
            return new ResponseMessage
            {
                status = true,
                data = provider.Chat.GetChatFromPresentation(Guid.Parse(presentationId)),
                message = "Get chat message successfully"
            };
        }
    }
}
