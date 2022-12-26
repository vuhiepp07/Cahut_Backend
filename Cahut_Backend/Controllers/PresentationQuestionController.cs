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
    public class PresentationQuestionController : BaseController
    {
        [HttpPost("/question/sendQuestion")]
        public ResponseMessage SendQuestion(object sendQuestionModel)
        {
            JObject objTemp = JObject.Parse(sendQuestionModel.ToString());
            string presentationId = (string)objTemp["presentationId"];
            string question = (string)objTemp["question"];
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
            int sendQuestionResult = provider.PresentationQuestion.SendQuestion(Guid.Parse(presentationId), question);
            return new ResponseMessage
            {
                status = sendQuestionResult > 0 ? true : false,
                data = null,
                message = sendQuestionResult > 0 ? "Send question successfully" : "Failed to send question"
            };
        }

        [HttpGet("/question/getQuestions")]
        public ResponseMessage GetQuestions(string presentationId)
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
                data = provider.PresentationQuestion.GetPresentationQuestions(Guid.Parse(presentationId)),
                message = "Get presentation questions successfully"
            };
        }

        [HttpPost("/question/upVoteQuestion")]
        public ResponseMessage UpvoteQuestion(string questionId)
        {
            Guid presentationId = provider.PresentationQuestion.GetPresentationId(questionId);
            if (!provider.Presentation.isPresentating(presentationId))
            {
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Presentation has not been presented yet"
                };
            }
            if (provider.PresentationQuestion.IsQuestionExisted(questionId))
            {
                int upvoteResult = provider.PresentationQuestion.UpVoteQuestion(questionId);
                return new ResponseMessage
                {
                    status = upvoteResult > 0 ? true : false,
                    data = null,
                    message = upvoteResult > 0 ? "Upvote question successfully" : "Failed to upvote question"
                };
            }
            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Question does not existed"
            };
        }

        [HttpPost("/question/UnUpVoteQuestion")]
        public ResponseMessage UnupvoteQuestion(string questionId)
        {
            Guid presentationId = provider.PresentationQuestion.GetPresentationId(questionId);
            if (!provider.Presentation.isPresentating(presentationId))
            {
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Presentation has not been presented yet"
                };
            }
            if (provider.PresentationQuestion.IsQuestionExisted(questionId))
            {
                int unupvoteResult = provider.PresentationQuestion.UnUpVoteQuestion(questionId);
                return new ResponseMessage
                {
                    status = unupvoteResult > 0 ? true : false,
                    data = null,
                    message = unupvoteResult > 0 ? " UnUpvote question successfully" : "Failed to unupvote question"
                };
            }
            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Question does not existed"
            };
        }

        [HttpPost("/question/markAnswered"), Authorize]
        public ResponseMessage MarkAnswered(string questionId, string groupId)
        {
            if (!provider.PresentationQuestion.IsQuestionExisted(questionId))
            {
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Question does not existed"
                };
            }

            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            Guid presentationId = provider.PresentationQuestion.GetPresentationId(questionId);
            bool isOwner = provider.Presentation.presentationExisted(presentationId, userId);
            if (!provider.Presentation.isPresentating(presentationId))
            {
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Presentation has not been presented yet"
                };
            }
            if (provider.Presentation.GetPresentationType(presentationId) == "public")
            {
                if (isOwner)
                {
                    int isAnswered = provider.PresentationQuestion.MarkedAsAnswered(questionId);
                    return new ResponseMessage
                    {
                        status = isAnswered > 0 ? true : false,
                        data = null,
                        message = isAnswered > 0 ? "Question has been marked as answered" : "Failed to marked answered question"
                    };
                }
            }
            if (provider.Presentation.GetPresentationType(presentationId) == "group") {
                string role = provider.Group.GetMemberRoleInGroup(userId, Guid.Parse(groupId));
                if(isOwner || role == "Co-owner")
                {
                    int isAnswered = provider.PresentationQuestion.MarkedAsAnswered(questionId);
                    return new ResponseMessage
                    {
                        status = isAnswered > 0 ? true : false,
                        data = null,
                        message = isAnswered > 0 ? "Question has been marked as answered" : "Failed to marked answered question"
                    };
                }
            }

            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Only owner or co-owner can marked question as answered"
            };
        }
    }
}
