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
        [HttpGet("/presentation/getslides")]
        public ResponseMessage GetPresentationSlide(string presentationId)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            bool isExisted = provider.Presentation.presentationExisted(Guid.Parse(presentationId), userId);
            if (isExisted)
            {
                List<object> res = provider.Presentation.GetPresentationSlides(Guid.Parse(presentationId));
                return new ResponseMessage
                {
                    status = false,
                    data = res,
                    message = "Get presentation slides success"
                };
            }
            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Presentation does not exist"
            };
        }

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
            string presentationId = (string)objTemp["presentationId"];
            //string oldName = (string)objTemp["oldName"];
            string newName = (string)objTemp["newName"];
            bool isExited = provider.Presentation.presentationExisted(Guid.Parse(presentationId), userId);
            //Presentation presentation = provider.Presentation.GetPresentationByNameAndTeacherId(oldName, userId);
            if(isExited == true)
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
                int updateResult = provider.Presentation.Update(Guid.Parse(presentationId), userId, newName);
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

        [HttpGet("/presentation/name")]
        public ResponseMessage GetPresentationName(string presentationId)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            try
            {
                Guid id = Guid.Parse(presentationId);
            }
            catch (Exception ex)
            {
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Get presentation failed, presentation does not exist"
                };
            }
            bool isExisted = provider.Presentation.presentationExisted(Guid.Parse(presentationId), userId);
            if (isExisted)
            {
                return new ResponseMessage
                {
                    status = true,
                    data = new { presentationName = provider.Presentation.GetPresentationName(Guid.Parse(presentationId)) },
                    message = "Get presentation name success"
                };
            }
            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Get presentation failed, presentation does not exist"
            };
        }


        [HttpPost("/presentation/addCollaborators"), Authorize]
        public ResponseMessage AddCollaborator(object addCollaboratorModel)
        {

            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            List<string> emails = new List<string>();
            JObject objTemp = JObject.Parse(addCollaboratorModel.ToString());
            JArray emailJarr = (JArray)objTemp["emailArray"];
            string presentationId = (string)objTemp["presentationId"];
            emails = emailJarr.ToObject<List<string>>();

            List<string> exisedEmail = new List<string>();

            bool addYourself = false;

            bool checkPresentation = provider.Presentation.presentationExisted(Guid.Parse(presentationId), userId);
            if (checkPresentation)
            {
                int collabAdded = 0;

                foreach (var email in emails)
                {
                    Guid user = provider.User.GetUserIdByUserEmail(email);
                    if (!user.Equals(userId))
                    {   
                        bool isAdded = provider.Presentation.AddCollaborators(Guid.Parse(presentationId), email);
                        if (isAdded)
                        {
                            collabAdded++;
                        }
                        else
                        {
                            exisedEmail.Add(email);
                        }
                    }
                    addYourself = true;
                }

                if (collabAdded == emails.Count)
                {
                    return new ResponseMessage
                    {
                        status = true,
                        data = null,
                        message = "Add collaborators successfully"
                    };
                }
                if (collabAdded != emails.Count && collabAdded > 0 )
                {
                    return new ResponseMessage
                    {
                        status = false,
                        data = exisedEmail,
                        message = "Some emails cannot be added"
                    };
                }

                if (collabAdded != emails.Count && collabAdded > 0 && addYourself)
                {
                    return new ResponseMessage
                    {
                        status = false,
                        data = exisedEmail,
                        message = "Cannot add yourself as collaborators, other emails had been added"
                    };
                }

                if (addYourself)
                {
                    return new ResponseMessage
                    {
                        status = false,
                        data = null,
                        message = "Cannot add your self as collaborator"
                    };
                }

                return new ResponseMessage
                {
                    status = false,
                    data = exisedEmail,
                    message = "Failed to add, all collaborators are added"
                };
                
            }
                
            
            

            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Add collaborators failed, please try again"
            };
        }

        [HttpPost("/presentation/removeCollaborator"), Authorize]
        public ResponseMessage RemoveCollaborators(CollaboratorModel collaboratorModel)
        {
            bool isEmailExisted = provider.User.CheckEmailExisted(collaboratorModel.email);
            if (!isEmailExisted)
            {
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "User does not existed",
                };
            }

            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            bool checkPresentation = provider.Presentation.presentationExisted(Guid.Parse(collaboratorModel.presentationId), userId);
            if (checkPresentation)
            {
                int removeResult = provider.Presentation.DeletCollaborators(Guid.Parse(collaboratorModel.presentationId), collaboratorModel.email);
                return new ResponseMessage
                {
                    status = removeResult > 0 ? true : false,
                    data = null,
                    message = removeResult > 0 ? "Delete collaborators successfully" : "Delete collaborators failed, please try again"
                };
            }
            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Delete collaborators failed, please try again"
            };
        }

        [HttpGet("/presentation/getCollaborators"), Authorize]
        public ResponseMessage GetCollaborators(string presentationId)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            Console.WriteLine(presentationId);

            bool checkPresentation = provider.Presentation.presentationExisted(Guid.Parse(presentationId), userId);
            if (checkPresentation)
            {
                object collaborators = provider.Presentation.GetCollaborators(Guid.Parse(presentationId));
                return new ResponseMessage
                {
                    status = collaborators is not null ? true : false,
                    data = collaborators,
                    message = collaborators is not null ? "Get collaborators successfully" : "Get collaborators failed, please try again"
                };
            }
            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Get collaborators failed, please try again"
            };
        }
    }
}
