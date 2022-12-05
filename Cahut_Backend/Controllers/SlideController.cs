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
    public class SlideController : BaseController
    {
        [HttpPost("/slide/create"), Authorize]
        public ResponseMessage Create(string presentationName)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            Presentation presentation = provider.Presentation.GetPresentationByNameAndTeacherId(presentationName, userId);
            if (presentation is not null)
            {
                string slideId = Helper.RandomString(8);
                while (provider.Slide.CheckSlideIdExisted(slideId) == true)
                {
                    slideId = Helper.RandomString(8);
                }
                int createResult = provider.Slide.Create(presentation.PresentationId, slideId);
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

        [HttpGet("/slide/delete"), Authorize]
        public ResponseMessage Delete(string slideId)
        {
            bool isExisted = provider.Slide.CheckSlideIdExisted(slideId);
            if(isExisted == true)
            {
                int deleteResult = provider.Slide.Delete(slideId);
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

        [HttpPost("/slide/add/question"), Authorize]
        public ResponseMessage AddQuestion(object question)
        {
            JObject ques = JObject.Parse(question.ToString());
            string content = (string)ques["content"];
            string type = (string)ques["type"];
            string slideId = (string)ques["slideId"];
            string questionId = Helper.RandomString(8);
            while(provider.Question.CheckExistedId(questionId) == true)
            {
                questionId = Helper.RandomString(8);
            };
            bool slideAlreadyHasQuestion = provider.Question.CheckSlideAlreadyHaveQues(slideId);
            if(slideAlreadyHasQuestion == false)
            {
                int addResult = provider.Question.AddToSlide(slideId, questionId, type, content);
                return new ResponseMessage
                {
                    status = addResult > 0 ? true : false,
                    data = addResult > 0 ? new { questionId = questionId} : null,
                    message = addResult > 0 ? "Add question to slide success" : "Add question to slide failed"
                };
            }
            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Add question to slide failed, slide already has question"
            };
    }

        [HttpGet("/slide/get/question"), Authorize]
        public ResponseMessage GetSlideQuestion(string slideId)
        {
            object question = provider.Question.GetSlideQuestion(slideId);
            return new ResponseMessage
            {
                status = true,
                data = question,
                message = question is not null ? "Get slide question success" : "Slide does not has any question"
            };
        }

        [HttpPost("/slide/update/question"), Authorize]
        public ResponseMessage UpdateQuestion(object question)
        {
            JObject ques = JObject.Parse(question.ToString());
            string content = (string)ques["content"];
            string type = (string)ques["type"];
            string slideId = (string)ques["slideId"];
            string questionId = (string)ques["questionId"];

            bool isExisted = provider.Question.CheckExistedId(questionId);
            if(isExisted == true)
            {
                int updateResult = provider.Question.Update(questionId, type, content);
                return new ResponseMessage
                {
                    status = updateResult > 0 ? true : false,
                    data = updateResult > 0 ? new { questionId = questionId, type = type, slideId = slideId, content = content } : null,
                    message = updateResult > 0 ? "Update question success" : "Update question failed"
                };
            }
            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Update failed, question does not exist"
            };
        }

        [HttpPost("/slide/delete/question"), Authorize]
        public ResponseMessage DeleteQuestion(string questionId)
        {
            bool isExisted = provider.Question.CheckExistedId(questionId);  
            if(isExisted == true)
            {
                int deleteResult = provider.Question.Delete(questionId);
                return new ResponseMessage
                {
                    status = deleteResult > 0 ? true : false,
                    data = null,
                    message = deleteResult > 0 ? "Delete question success" : "Delete question failed"
                };
            }
            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Delete failed, question does not exist"
            };
        }

        [HttpPost("/slide/add/answer"), Authorize]
        public ResponseMessage AddAnswer(object answer)
        {
            JObject ans = JObject.Parse(answer.ToString());
            string content = (string)ans["content"];
            string questionId = (string)ans["questionId"];

            bool questionExisted = provider.Question.CheckExistedId(questionId);
            if (questionExisted == true)
            {
                string answerId = Helper.RandomString(8);
                while (provider.Answer.AnswerIdExisted(answerId) == true)
                {
                    answerId = Helper.RandomString(8);
                };
                int addResult = provider.Answer.Add(answerId, questionId, content);
                return new ResponseMessage
                {
                    status = addResult > 0 ? true : false,
                    data = addResult > 0 ? new { answerId = answerId, content = content }:null,
                    message = addResult > 0 ? "Add answer for question success" : "Add answer failed",
                };
            }
            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Add answer failed, question does not exist"
            };
        }

        [HttpPost("/slide/update/answer"), Authorize]
        public ResponseMessage UpdateAnswer(object answer)
        {
            JObject ans = JObject.Parse(answer.ToString());
            string content = (string)ans["content"];
            string answerId = (string)ans["answerId"];
            bool answerExisted = provider.Answer.AnswerIdExisted(answerId);
            if(answerExisted == true)
            {
                int updateResult = provider.Answer.Update(answerId, content);
                if(updateResult > 0)
                {
                    return new ResponseMessage
                    {
                        status = true,
                        data = new { answerId = answerId, content = content },
                        message = "Update answer success"
                    };
                }
                else
                {
                    return new ResponseMessage
                    {
                        status = false,
                        data = null,
                        message = "Update answer failed"
                    };
                }
            }
            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Update answer failed, answer does not existed"
            };
        }

        [HttpGet("/slide/delete/answer"), Authorize]
        public ResponseMessage DeleteAnswer(string answerId)
        {
            bool answerExisted = provider.Answer.AnswerIdExisted(answerId);
            if(answerExisted == true)
            {
                int deleteResult = provider.Answer.Delete(answerId);
                if(deleteResult > 0)
                {
                    return new ResponseMessage
                    {
                        status = true,
                        data = null,
                        message = "Delete answer success"
                    };
                }
                else
                {
                    return new ResponseMessage
                    {
                        status = false,
                        data = null,
                        message = "Delete answer failed"
                    };
                }
            }
            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Delete answer failed, answer does not exist"
            };
        }

        [HttpGet("/slide/get/answers"), Authorize]
        public ResponseMessage GetQuestionAnswers(string questionId)
        {
            bool questionExist = provider.Question.CheckExistedId(questionId);
            if(questionExist == true)
            {
                List<object> answers = provider.Answer.GetQuestionAnswer(questionId);
                return new ResponseMessage
                {
                    status = true,
                    data = answers,
                    message = "Get question's answers success"
                };
            }
            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Get answers failed, question does not exist"
            };
        }

        [HttpPost("slide/update"), Authorize]
        public ResponseMessage UpdateSlide(object info)
        {
            JObject objTemp = JObject.Parse(info.ToString());
            JObject question = JObject.Parse(objTemp["question"].ToString());
            JArray answers = (JArray)objTemp["answers"];
            List<JObject> answerList = answers.ToObject<List<JObject>>();
            string questionId = question["questionId"].ToString();

            if (question["isEdited"].ToString() == "true")
            {
                string slideId = question["slideId"].ToString();

                string content = question["content"].ToString();
                string type = question["type"].ToString();
                bool questionExisted = provider.Question.CheckExistedId(questionId);
                int result;
                if (questionExisted)
                {
                    result = provider.Question.Update(questionId, type, content);
                }
                else
                {
                    bool slideAlreadyHasQuestion = provider.Question.CheckSlideAlreadyHaveQues(slideId);
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
                    while (provider.Question.CheckExistedId(questionId) == true)
                    {
                        questionId = Helper.RandomString(8);
                    };
                    result = provider.Question.AddToSlide(slideId, questionId, type, content);
                }

            }
            for(int i =0;i < answerList.Count; i++)
            {
                if (answerList[i]["isEdited"].ToString() == "true")
                {
                    int result;
                    string answerId = answerList[i]["answerId"].ToString();
                    string content = answerList[i]["content"].ToString();
                    bool answerExisted = provider.Answer.AnswerIdExisted(answerId);
                    if (answerExisted)
                    {
                        result = provider.Answer.Update(answerId, content);
                    }
                    else
                    {
                        answerId = Helper.RandomString(8);
                        while (provider.Answer.AnswerIdExisted(answerId) == true)
                        {
                            answerId = Helper.RandomString(8);
                        };
                        result = provider.Answer.Add(answerId, questionId, content);
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
    }
}
