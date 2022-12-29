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
    public class MultipleChoiceSlideController : BaseController
    {

        [HttpPost("/slide/multiplechoice/create"), Authorize]
        public ResponseMessage Create(string presentationId)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            //Presentation presentation = provider.Presentation.GetPresentationByNameAndTeacherId(presentationName, userId);
            bool isExisted = provider.Presentation.presentationExisted(Guid.Parse(presentationId), userId);
            bool isCollab = provider.Presentation.isCollaborator(Guid.Parse(presentationId), userId);
            if (isExisted == true || isCollab)
            {
                string slideId = Helper.RandomString(8);
                while (provider.MultipleChoiceSlide.CheckSlideIdExisted(slideId) == true)
                {
                    slideId = Helper.RandomString(8);
                }
                int createResult = provider.MultipleChoiceSlide.CreateMultipleChoiceSlide(Guid.Parse(presentationId), slideId);
                return new ResponseMessage
                {
                    status = createResult > 0 ? true : false,
                    data = createResult > 0? new { slideId = slideId } :null,
                    message = createResult > 0 ? "Create slide success" : "Create slide failed"
                };
            }
            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Create slide failed, presentation does not exist",
            };
        }

        [HttpGet("/slide/multiplechoice/delete"), Authorize]
        public ResponseMessage Delete(string slideId)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            Guid presentationId = provider.MultipleChoiceSlide.GetPresentationId(slideId);
            bool isExisted = provider.MultipleChoiceSlide.CheckSlideIdExisted(slideId);
            bool isCollab = provider.Presentation.isCollaborator(presentationId, userId);
            if (isExisted == true || isCollab )
            {
                int deleteResult = provider.MultipleChoiceSlide.Delete(slideId);
                //string questionId = provider.Question.DeleteWithSlide(slideId);
                //provider.Answer.DeleteWithQuestion(questionId);
                return new ResponseMessage
                {
                    status = deleteResult > 0 ? true : false,
                    data = null,
                    message = deleteResult > 0 ? "Delete slide success" : "Delete slide failed"
                };
            }
            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Delete slide failed, slide does not existed"
            };
        }

        //    [HttpPost("/slide/multiplechoice/add/question"), Authorize]
        //    public ResponseMessage AddQuestion(object question)
        //    {
        //        JObject ques = JObject.Parse(question.ToString());
        //        string content = (string)ques["content"];
        //        string type = (string)ques["type"];
        //        string slideId = (string)ques["slideId"];
        //        string questionId = Helper.RandomString(8);
        //        while(provider.MultipleChoiceQuestion.CheckExistedId(questionId) == true)
        //        {
        //            questionId = Helper.RandomString(8);
        //        };
        //        bool slideAlreadyHasQuestion = provider.MultipleChoiceQuestion.CheckSlideAlreadyHaveQues(slideId);
        //        if(slideAlreadyHasQuestion == false)
        //        {
        //            int addResult = provider.MultipleChoiceQuestion.AddToSlide(slideId, questionId, type, content);
        //            return new ResponseMessage
        //            {
        //                status = addResult > 0 ? true : false,
        //                data = addResult > 0 ? new { questionId = questionId} : null,
        //                message = addResult > 0 ? "Add question to slide success" : "Add question to slide failed"
        //            };
        //        }
        //        return new ResponseMessage
        //        {
        //            status = false,
        //            data = null,
        //            message = "Add question to slide failed, slide already has question"
        //        };
        //}

        [HttpGet("/slide/multiplechoice/get/question")]
        public ResponseMessage GetSlideQuestion(string slideId)
        {
            object question = provider.MultipleChoiceQuestion.GetSlideQuestion(slideId);
            return new ResponseMessage
            {
                status = true,
                data = question,
                message = question is not null ? "Get slide question success" : "Slide does not has any question"
            };
        }

        //[HttpPost("/slide/multiplechoice/update/question"), Authorize]
        //public ResponseMessage UpdateQuestion(object question)
        //{
        //    JObject ques = JObject.Parse(question.ToString());
        //    string content = (string)ques["content"];
        //    string type = (string)ques["type"];
        //    string slideId = (string)ques["slideId"];
        //    string questionId = (string)ques["questionId"];

        //    bool isExisted = provider.MultipleChoiceQuestion.CheckExistedId(questionId);
        //    if(isExisted == true)
        //    {
        //        int updateResult = provider.MultipleChoiceQuestion.Update(questionId, type, content);
        //        return new ResponseMessage
        //        {
        //            status = updateResult > 0 ? true : false,
        //            data = updateResult > 0 ? new { questionId = questionId, type = type, slideId = slideId, content = content } : null,
        //            message = updateResult > 0 ? "Update question success" : "Update question failed"
        //        };
        //    }
        //    return new ResponseMessage
        //    {
        //        status = false,
        //        data = null,
        //        message = "Update failed, question does not exist"
        //    };
        //}

        //[HttpPost("/slide/multiplechoice/delete/question"), Authorize]
        //public ResponseMessage DeleteQuestion(string questionId)
        //{
        //    bool isExisted = provider.MultipleChoiceQuestion.CheckExistedId(questionId);  
        //    if(isExisted == true)
        //    {
        //        int deleteResult = provider.MultipleChoiceQuestion.Delete(questionId);
        //        return new ResponseMessage
        //        {
        //            status = deleteResult > 0 ? true : false,
        //            data = null,
        //            message = deleteResult > 0 ? "Delete question success" : "Delete question failed"
        //        };
        //    }
        //    return new ResponseMessage
        //    {
        //        status = false,
        //        data = null,
        //        message = "Delete failed, question does not exist"
        //    };
        //}

        //[HttpPost("/slide/multiplechoice/add/option"), Authorize]
        //public ResponseMessage AddOption(object option)
        //{
        //    JObject op = JObject.Parse(option.ToString());
        //    string content = (string)op["content"];
        //    string questionId = (string)op["questionId"];

        //    bool questionExisted = provider.MultipleChoiceQuestion.CheckExistedId(questionId);
        //    if (questionExisted == true)
        //    {
        //        string answerId = Helper.RandomString(8);
        //        while (provider.MultipleChoiceOption.MultipleChoiceOptionIdExisted(answerId) == true)
        //        {
        //            answerId = Helper.RandomString(8);
        //        };
        //        int addResult = provider.MultipleChoiceOption.Add(answerId, questionId, content);
        //        return new ResponseMessage
        //        {
        //            status = addResult > 0 ? true : false,
        //            data = addResult > 0 ? new { answerId = answerId, content = content }:null,
        //            message = addResult > 0 ? "Add option for question success" : "Add option failed",
        //        };
        //    }
        //    return new ResponseMessage
        //    {
        //        status = false,
        //        data = null,
        //        message = "Add answer failed, question does not exist"
        //    };
        //}

        //[HttpPost("/slide/multiplechoice/update/option"), Authorize]
        //public ResponseMessage UpdateOption(object option)
        //{
        //    JObject ans = JObject.Parse(option.ToString());
        //    string content = (string)ans["content"];
        //    string optionId = (string)ans["optionId"];
        //    bool optionExisted = provider.MultipleChoiceOption.MultipleChoiceOptionIdExisted(optionId);
        //    if(optionExisted == true)
        //    {
        //        int updateResult = provider.MultipleChoiceOption.Update(optionId, content);
        //        if(updateResult > 0)
        //        {
        //            return new ResponseMessage
        //            {
        //                status = true,
        //                data = new { optionId = optionId, content = content },
        //                message = "Update option success"
        //            };
        //        }
        //        else
        //        {
        //            return new ResponseMessage
        //            {
        //                status = false,
        //                data = null,
        //                message = "Update option failed"
        //            };
        //        }
        //    }
        //    return new ResponseMessage
        //    {
        //        status = false,
        //        data = null,
        //        message = "Update answer failed, answer does not existed"
        //    };
        //}

        [HttpGet("/slide/multiplechoice/delete/option"), Authorize]
        public ResponseMessage DeleteOption(string optionId)
        {

            bool optionExisted = provider.MultipleChoiceOption.MultipleChoiceOptionIdExisted(optionId);
            if(optionExisted == true)
            {
                int deleteResult = provider.MultipleChoiceOption.Delete(optionId);
                if(deleteResult > 0)
                {
                    return new ResponseMessage
                    {
                        status = true,
                        data = null,
                        message = "Delete option success"
                    };
                }
                else
                {
                    return new ResponseMessage
                    {
                        status = false,
                        data = null,
                        message = "Delete option failed"
                    };
                }
            }
            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Delete option failed, option does not exist"
            };
        }

        [HttpGet("/slide/multiplechoice/get/options")]
        public ResponseMessage GetQuestionOptions(string questionId)
        {
            bool questionExist = provider.MultipleChoiceQuestion.CheckExistedId(questionId);
            if(questionExist == true)
            {
                List<object> options = provider.MultipleChoiceOption.GetMultipleChoiceQuestionOptions(questionId);
                return new ResponseMessage
                {
                    status = true,
                    data = options,
                    message = "Get question's options success"
                };
            }
            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Get options failed, question does not exist"
            };
        }

        [HttpPost("/slide/multiplechoice/update"), Authorize]
        public ResponseMessage UpdateSlide(object info)
        {
            JObject objTemp = JObject.Parse(info.ToString());
            JObject question = JObject.Parse(objTemp["question"].ToString());
            JArray options = (JArray)objTemp["options"];
            List<JObject> optionList = options.ToObject<List<JObject>>();
            string questionId = question["questionId"].ToString();

            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            if (question["isEdited"].ToString() == "true")
            {
                string slideId = question["slideId"].ToString();

                string content = question["content"].ToString();
                string type = question["type"].ToString();
                bool questionExisted = provider.MultipleChoiceQuestion.CheckExistedId(questionId);
                int result;
                if (questionExisted)
                {
                    result = provider.MultipleChoiceQuestion.Update(questionId, type, content);
                }
                else
                {
                    bool slideAlreadyHasQuestion = provider.MultipleChoiceQuestion.CheckSlideAlreadyHaveQues(slideId);
                    if (slideAlreadyHasQuestion == true)
                    {
                        return new ResponseMessage
                        {
                            status = false,
                            data = null,
                            message = "Slide already have question, can not add a new one"
                        };
                    }
                        questionId = Helper.RandomString(8);
                    while (provider.MultipleChoiceQuestion.CheckExistedId(questionId) == true)
                    {
                        questionId = Helper.RandomString(8);
                    };
                    result = provider.MultipleChoiceQuestion.AddToSlide(slideId, questionId, type, content);
                }

            }
            for(int i =0;i < optionList.Count; i++)
            {
                if (optionList[i]["isEdited"].ToString() == "true")
                {
                    int result;
                    string optionId = optionList[i]["optionId"].ToString();
                    string content = optionList[i]["content"].ToString();
                    bool optionExisted = provider.MultipleChoiceOption.MultipleChoiceOptionIdExisted(optionId);
                    if (optionExisted)
                    {
                        result = provider.MultipleChoiceOption.Update(optionId, content);
                    }
                    else
                    {
                        optionId = Helper.RandomString(8);
                        while (provider.MultipleChoiceOption.MultipleChoiceOptionIdExisted(optionId) == true)
                        {
                            optionId = Helper.RandomString(8);
                        };
                        result = provider.MultipleChoiceOption.Add(optionId, questionId, content);
                    }
                }
            }
            return new ResponseMessage
            {
                status = true,
                data = null,
                message = "Change saved"
            };
        }

        [HttpGet("/slide/multiplechoice/checkSubmitted")]
        public ResponseMessage CheckSubmitted(string questionId)
        {
            string userId = Guid.Empty.ToString();
            var accessToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            if (accessToken != null && accessToken != string.Empty)
            {
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                userId = handler.ReadJwtToken(accessToken).Claims.First(claim => claim.Type == "nameid").Value;
                bool isSubmitted = provider.MultipleChoiceQuestion.IsUserSubmitted(Guid.Parse(userId), questionId);
                return new ResponseMessage
                {
                    status = isSubmitted ? false : true,
                    data = null,
                    message = isSubmitted ? "User has submitted this question" : "User can submit this question"
                };
            }
            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Record only stored in group present"
            };
        }

        [HttpPost("/slide/multiplechoice/submitchoice")]
        public ResponseMessage SubmitChoice(string optionId)
        {


            bool optionExist = provider.MultipleChoiceOption.MultipleChoiceOptionIdExisted(optionId);
            if (optionExist)
            {
                string userId = Guid.Empty.ToString();
                var accessToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
                if (accessToken != null && accessToken != string.Empty)
                {
                    JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                    userId = handler.ReadJwtToken(accessToken).Claims.First(claim => claim.Type == "nameid").Value;
                }

                string questionId = provider.MultipleChoiceOption.GetMultipleChoiceQuestion(optionId);

                if (!provider.MultipleChoiceQuestion.IsUserSubmitted(Guid.Parse(userId), questionId))
                {

                    int increaseResult = provider.MultipleChoiceOption.IncreaseByOne(optionId, Guid.Parse(userId));
                    if (increaseResult > 0)
                    {
                        return new ResponseMessage
                        {
                            status = true,
                            data = new { currentSelected = increaseResult },
                            message = "Submit choice success"
                        };
                    }
                }

                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Submit choice failed"
                };
            }
            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Submit choice failed, option does not exist"
            };
        }
    }
}
