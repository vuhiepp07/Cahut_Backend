using Cahut_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
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
            string userId = Guid.Empty.ToString();
            var accessToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            if (accessToken != null && accessToken != string.Empty)
            {
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                userId = handler.ReadJwtToken(accessToken).Claims.First(claim => claim.Type == "nameid").Value;
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
                data = provider.PresentationQuestion.GetPresentationQuestions(Guid.Parse(presentationId), Guid.Parse(userId)),
                message = "Get presentation questions successfully"
            };
        }

        [HttpGet("/question/getAnsweredQuestions")]
        public ResponseMessage GetAnsweredQuestion(string presentationId)
        {
            string userId = Guid.Empty.ToString();
            var accessToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            if (accessToken != null && accessToken != string.Empty)
            {
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                userId = handler.ReadJwtToken(accessToken).Claims.First(claim => claim.Type == "nameid").Value;
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
                data = provider.PresentationQuestion.GetAnsweredQuestions(Guid.Parse(presentationId), Guid.Parse(userId)),
                message = "Get presentation questions successfully"
            };
        }

        [HttpGet("/question/getUnAnsweredQuestions")]
        public ResponseMessage GetUnansweredQuestion(string presentationId)
        {
            string userId = Guid.Empty.ToString();
            var accessToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            if (accessToken != null && accessToken != string.Empty)
            {
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                userId = handler.ReadJwtToken(accessToken).Claims.First(claim => claim.Type == "nameid").Value;
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
                data = provider.PresentationQuestion.GetUnAnsweredQuestions(Guid.Parse(presentationId), Guid.Parse(userId)),
                message = "Get presentation questions successfully"
            };
        }

        [HttpGet("/question/get/questionSortedByVote")]
        public ResponseMessage GetQuestionSortedByVote(string presentationId, string sortType)
        {
            string userId = Guid.Empty.ToString();
            var accessToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            if (accessToken != null && accessToken != string.Empty)
            {
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                userId = handler.ReadJwtToken(accessToken).Claims.First(claim => claim.Type == "nameid").Value;
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
            if (provider.Presentation.GetPresentationName(Guid.Parse(presentationId)) == null)
            {
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Presentation does not existed",
                };
            }
            if (sortType != "Descending" && sortType != "Ascending")
            {
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Invalid sort type, sort type must be Descending or Ascending"
                };
            }
            return new ResponseMessage
            {
                status = true,
                data = provider.PresentationQuestion.GetQuestionsSortedByVote(Guid.Parse(presentationId), Guid.Parse(userId), sortType),
                message = "Get presentation questions successfully"
            };
        }

        [HttpGet("/question/get/questionSortedByTime")]
        public ResponseMessage GetQuestionSortedByTime(string presentationId, string sortType)
        {
            string userId = Guid.Empty.ToString();
            var accessToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            if (accessToken != null && accessToken != string.Empty)
            {
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                userId = handler.ReadJwtToken(accessToken).Claims.First(claim => claim.Type == "nameid").Value;
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
            if (provider.Presentation.GetPresentationName(Guid.Parse(presentationId)) == null)
            {
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Presentation does not existed",
                };
            }
            if (sortType != "Descending" && sortType != "Ascending")
            {
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Invalid sort type, sort type must be Descending or Ascending"
                };
            }
            return new ResponseMessage
            {
                status = true,
                data = provider.PresentationQuestion.GetQuestionsSortedByTime(Guid.Parse(presentationId), Guid.Parse(userId), sortType),
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
                string userId = Guid.Empty.ToString();
                var accessToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
                if (accessToken != null && accessToken != string.Empty)
                {
                    JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                    userId = handler.ReadJwtToken(accessToken).Claims.First(claim => claim.Type == "nameid").Value;
                }

                if (!provider.PresentationQuestion.IsVote(questionId, Guid.Parse(userId)))
                {
                    int upvoteResult = provider.PresentationQuestion.UpVoteQuestion(questionId, Guid.Parse(userId));
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
                    message = "User has already vote"
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
                string userId = Guid.Empty.ToString();
                var accessToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
                if (accessToken != null && accessToken != string.Empty)
                {
                    JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                    userId = handler.ReadJwtToken(accessToken).Claims.First(claim => claim.Type == "nameid").Value;
                }
                int unupvoteResult = provider.PresentationQuestion.UnUpVoteQuestion(questionId, Guid.Parse(userId));
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

        [HttpPost("/question/updateQuestionAnswered"), Authorize]
        public ResponseMessage UpdateQuestionAnswered(string questionId, string groupId)
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
                    int isAnswered = provider.PresentationQuestion.UpdateQuestionAnswered(questionId);
                    return new ResponseMessage
                    {
                        status = isAnswered > 0 ? true : false,
                        data = null,
                        message = isAnswered > 0 ? "Question answered status has been updated" : "Failed to update answered question status"
                    };
                }
            }
            if (provider.Presentation.GetPresentationType(presentationId) == "group") {
                string role = provider.Group.GetMemberRoleInGroup(userId, Guid.Parse(groupId));
                if(isOwner || role == "Co-owner")
                {
                    int isAnswered = provider.PresentationQuestion.UpdateQuestionAnswered(questionId);
                    return new ResponseMessage
                    {
                        status = isAnswered > 0 ? true : false,
                        data = null,
                        message = isAnswered > 0 ? "Question answered status has been updated" : "Failed to update answered question status"
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
