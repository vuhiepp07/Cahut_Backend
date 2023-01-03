using Cahut_Backend.Models;
using Cahut_Backend.SignalR.Hubs;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;


namespace Cahut_Backend.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class PresentationController : BaseController
    {
        private readonly IHubContext<SlideHub> _hubContext;

        public PresentationController(IHubContext<SlideHub> hubContext) : base()
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }

        [HttpGet("/presentation/getslides"), Authorize]
        public ResponseMessage GetPresentationSlide(string presentationId)
        {

            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            bool isExisted = provider.Presentation.presentationExisted(Guid.Parse(presentationId), userId);
            bool isCollab = provider.Presentation.isCollaborator(Guid.Parse(presentationId), userId);
            if (isExisted || isCollab)
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
            if (isExisted == false)
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
            if (isExited == true)
            {
                bool newNameExisted = provider.Presentation.CheckExisted(userId, newName);
                if (newNameExisted == true)
                {
                    return new ResponseMessage
                    {
                        status = false,
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

        [HttpGet("/presentation/info"), Authorize]
        public ResponseMessage GetPresentationInfo(string presentationId)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            bool canParse = true;
            try
            {
                Guid.Parse(presentationId);
            }
            catch
            {
                canParse = false;

            }
            if (canParse is false)
            {
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Guid format is invalid"
                };
            }
            bool isCollab = provider.Presentation.isCollaborator(Guid.Parse(presentationId), userId);
            bool isOwner = provider.Presentation.presentationExisted(Guid.Parse(presentationId), userId);
            if (isCollab || isOwner)
            {
                return new ResponseMessage
                {
                    status = true,
                    data = new
                    {
                        Role = isCollab ? "Collaborator" : "Owner",
                        PresentatingType = provider.Presentation.GetPresentationType(Guid.Parse(presentationId)),
                        IsBeingPresented = provider.Presentation.isPresentating(Guid.Parse(presentationId)),
                        presentationName = provider.Presentation.GetPresentationName(Guid.Parse(presentationId)),
                        GroupId = provider.Presentation.GetPresentGroup(Guid.Parse(presentationId)),
                    },
                    message = "Get presentation info successfully"
                };
            }
            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Only owner and collaborators can get presentation info"
            };
        }

        [HttpPost("/presentation/addCollaborator"), Authorize]
        public ResponseMessage AddCollaborator(object addCollaboratorModel)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            JObject objTemp = JObject.Parse(addCollaboratorModel.ToString());
            string presentationId = (string)objTemp["presentationId"];
            string email = (string)objTemp["email"];

            //check add yourself
            Guid user = provider.User.GetUserIdByUserEmail(email);
            if (user.Equals(userId))
            {
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Cannot add yourself as collaborator"
                };
            }
            if (!provider.User.CheckEmailExisted(email))
            {
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Email does not existed"
                };
            }
            bool addResult = provider.Presentation.AddCollaborators(Guid.Parse(presentationId), email);
            return new ResponseMessage
            {
                status = addResult ? true : false,
                data = null,
                message = addResult ? "Add collaborator successfully" : "Failed to add collaborator, user is already a collaborator"
            };

        }

        

        [HttpPost("/presentation/addCollaborators"), Authorize]
        public ResponseMessage AddCollaborators(object addCollaboratorModel)
        {

            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            List<string> emails = new List<string>();
            JObject objTemp = JObject.Parse(addCollaboratorModel.ToString());
            JArray emailJarr = (JArray)objTemp["emailArray"];
            string presentationId = (string)objTemp["presentationId"];
            emails = emailJarr.ToObject<List<string>>();

            List<string> exisedEmail = new List<string>();

            string errorMessage = "";

            bool addYourself = false;

            bool checkPresentation = provider.Presentation.presentationExisted(Guid.Parse(presentationId), userId);
            if (checkPresentation)
            {
                int collabAdded = 0;

                foreach (var email in emails)
                {
                    bool isEmailExisted = provider.User.CheckEmailExisted(email);
                    if (!isEmailExisted)
                    {
                        errorMessage += " There are some email does not existed in system ";
                    }
                    Guid user = provider.User.GetUserIdByUserEmail(email);
                    if (user.Equals(userId))
                    {
                        addYourself = true;
                        errorMessage += " Cannot add your self ";
                    }
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

                if (collabAdded == emails.Count)
                {
                    return new ResponseMessage
                    {
                        status = true,
                        data = null,
                        message = "Add collaborators successfully"
                    };
                }
                if (collabAdded != emails.Count && collabAdded > 0)
                {
                    return new ResponseMessage
                    {
                        status = false,
                        data = exisedEmail,
                        message = "Some emails cannot be added" + errorMessage
                    };
                }

                if (collabAdded != emails.Count && collabAdded > 0)
                {
                    return new ResponseMessage
                    {
                        status = false,
                        data = exisedEmail,
                        message = "Cannot add yourself as collaborators, other emails had been added" + errorMessage
                    };
                }


                return new ResponseMessage
                {
                    status = false,
                    data = exisedEmail,
                    message = "Failed to add " + errorMessage
                };

            }

            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Add collaborators failed, presentation does not existed"
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

        [HttpGet("/presentation/getCollabPresentations")]
        public ResponseMessage GetCollabPresentations()
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return new ResponseMessage
            {
                status = true,
                data = provider.Presentation.GetCollaboratorPresentation(userId),
                message = "Get collab presentations successfully"
            };
        }

        [HttpGet("/presentation/groupPresent")]
        public ResponseMessage GroupPresent(string presentationId, string groupName)
        {
            Models.Group group = provider.Group.GetGroupByName(groupName);
            if (group == null)
            {
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Group name does not existed"
                };
            }
            Guid groupId = provider.Group.GetGroupByName(groupName).GroupId;
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (!provider.Group.AlreadyJoinedGroup(userId, groupId)) {
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "You are not in this group, ask group owner to join"
                };
            }
            bool isCollab = provider.Presentation.isCollaborator(Guid.Parse(presentationId), userId);
            bool isOwner = provider.Presentation.presentationExisted(Guid.Parse(presentationId), userId);
            int presentGroup = 0;
            if (isCollab || isOwner)
            {
                if (provider.Presentation.isPresentating(Guid.Parse(presentationId)))
                {
                    return new ResponseMessage
                    {
                        status = false,
                        data = null,
                        message = "presention is being presented"
                    };
                }
                presentGroup = provider.Presentation.StartGroupPresentation(Guid.Parse(presentationId), groupId);
            }

           
            List<string> emails = provider.Group.GetGrpEmails(groupId);
            foreach (string email in emails)
            {
                Guid user = provider.User.GetUserIdByUserEmail(email);
                string role = provider.Group.GetMemberRoleInGroup(user, groupId);
                string presentLink = Helper.TestingLink;
                if (role == "Owner" || role == "Co-owner")
                {
                    presentLink += "/presentation/present/" + presentationId;
                }
                else
                {
                    presentLink += "/view/" + presentationId;
                }
                foreach (var connectionId in SlideHub._userConnections.GetConnections(email))
                {
                    
                    Console.WriteLine(connectionId);
                    _hubContext.Clients.Client(connectionId).SendAsync("NotifyGroup", new { grpName = groupName, link = presentLink });
                }
            }
            return new ResponseMessage
            {
                status = true,
                data = emails,
                message = "presenting in group"
            };

            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Only owner or collaborators can start presentation"
            };
        }

        [HttpGet("/presentation/publicPresent")]
        public ResponseMessage PublicPresent(string presentationId)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            bool isCollab = provider.Presentation.isCollaborator(Guid.Parse(presentationId), userId);
            bool isOwner = provider.Presentation.presentationExisted(Guid.Parse(presentationId), userId);
            if (isCollab || isOwner)
            {
                if (provider.Presentation.isPresentating(Guid.Parse(presentationId)))
                {
                    return new ResponseMessage
                    {
                        status = false,
                        data = null,
                        message = "Presentation is being presented"
                    };
                }
                int isPresent = provider.Presentation.StartPublicPresentation(Guid.Parse(presentationId));
                return new ResponseMessage
                {
                    status = true,
                    data = null,
                    message = "presenting in public"
                };
            }

            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Only owner or collaborators can start presentation"
            };
        }

        [HttpGet("/presentation/endPresentation")]
        public ResponseMessage EndPresentation(string presentationId)
        {
            if (!provider.Presentation.isPresentating(Guid.Parse(presentationId)))
            {
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Presentation has been already ended"
                };
            }
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            bool isCollab = provider.Presentation.isCollaborator(Guid.Parse(presentationId), userId);
            bool isOwner = provider.Presentation.presentationExisted(Guid.Parse(presentationId), userId);
            if (provider.Presentation.GetPresentationType(Guid.Parse(presentationId)) == "group")
            {
                Guid presentingGroup = provider.Presentation.GetPresentGroup(Guid.Parse(presentationId));
                if (provider.Group.GetMemberRoleInGroup(userId, presentingGroup) == "Co-owner" || provider.Group.GetMemberRoleInGroup(userId, presentingGroup) == "Owner")
                {
                    int isEnd = provider.Presentation.EndPresentation(Guid.Parse(presentationId));
                    return new ResponseMessage
                    {
                        status = isEnd > 0 ? true : false,
                        data = null,
                        message = isEnd > 0 ? "End a presentation" : "Failed to end presentation"
                    };
                }
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Only owner and Co-owner can end presentation"
                };
            }
            if ((provider.Presentation.GetPresentationType(Guid.Parse(presentationId)) == "public"))
            {
                int isEnd = provider.Presentation.EndPresentation(Guid.Parse(presentationId));
                return new ResponseMessage
                {
                    status = isEnd > 0 ? true : false,
                    data = null,
                    message = isEnd > 0 ? "End a presentation" : "Failed to end presentation"
                };
            }
            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Invalid presenting type"
            };
        }

        [HttpGet("/presentation/public/currentSlide")]
        public ResponseMessage GetCurrentSlide(string presentationId)
        {
            string presentType = provider.Presentation.GetPresentationType(Guid.Parse(presentationId));
            if (presentType == "public")
            {
                if (provider.Presentation.isPresentating(Guid.Parse(presentationId)))
                {
                    Slide currentSlide = provider.Presentation.GetCurrentSlide(Guid.Parse(presentationId));
                    return new ResponseMessage
                    {
                        status = true,
                        data = new
                        {
                            slideId = currentSlide is not null ? currentSlide.SlideId : null,
                            slideType = currentSlide is not null ? currentSlide.SlideType : null,
                        },
                        message = "Get current slide successfully"
                    };
                }
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Presentation is not presented"
                };
            }
            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Presentation is not presented in public"
            };
        }

        [HttpGet("/presentation/group/currentSlide"), Authorize]
        public ResponseMessage GetCurrentGroupSlide(string presentationId, string groupId)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            bool isPresenting = provider.Presentation.isPresentating(Guid.Parse(presentationId));
            if (isPresenting)
            {
                bool isJoinedGroup = provider.Group.AlreadyJoinedGroup(userId, Guid.Parse(groupId));
                bool isOwner = provider.Presentation.presentationExisted(Guid.Parse(presentationId), userId);
                if (isOwner || isJoinedGroup)
                {
                    Slide currentSlide = provider.Presentation.GetCurrentSlide(Guid.Parse(presentationId));
                    return new ResponseMessage
                    {
                        status = true,
                        data = new
                        {
                            slideId = currentSlide is not null ? currentSlide.SlideId : null,
                            slideType = currentSlide is not null ? currentSlide.SlideType : null,
                        },
                        message = "Get current slide successfully"
                    };
                }
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Only group member can view presentation"
                };
            }
            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Presentation is not being presented"
            };
        }

        [HttpGet("/presentation/getNextSlide"), Authorize]
        public ResponseMessage GetNextSlide(string presentationId, string groupId)
        {
            string presentationType = provider.Presentation.GetPresentationType(Guid.Parse(presentationId));
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            bool isOwner = provider.Presentation.presentationExisted(Guid.Parse(presentationId), userId);
            if (presentationType == "public")
            {
                if (isOwner)
                {
                    if (!provider.Presentation.isPresentating(Guid.Parse(presentationId)))
                    {
                        return new ResponseMessage
                        {
                            status = false,
                            data = null,
                            message = "Presentation is not presented"
                        };
                    }
                    object nextSlide = provider.Presentation.GetNextSlide(Guid.Parse(presentationId));
                    return new ResponseMessage
                    {
                        status = true,
                        data = nextSlide,
                        message = nextSlide != null ? "Get next slide successfully" : "Has reached end of presentation"
                    };
                }
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Only presentation onwner can get next slide"
                };
            }
            if (presentationType == "group")
            {
                string userRole = provider.Group.GetMemberRoleInGroup(userId, Guid.Parse(groupId));
                bool isJoinedGroup = provider.Group.AlreadyJoinedGroup(userId, Guid.Parse(groupId));
                if (isJoinedGroup && (userRole == "Co-owner" || userRole == "Owner"))
                {
                    if (!provider.Presentation.isPresentating(Guid.Parse(presentationId)))
                    {
                        return new ResponseMessage
                        {
                            status = false,
                            data = null,
                            message = "Presentation is not presented"
                        };
                    }
                    object nextSlide = provider.Presentation.GetNextSlide(Guid.Parse(presentationId));
                    return new ResponseMessage
                    {
                        status = true,
                        data = nextSlide,
                        message = nextSlide != null ? "Get next slide successfully" : "Has reached end of presentation"
                    };
                }
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Only presentation onwner, co-owner can get next slide"
                };
            }
            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Invalid presentation type"
            };
        }

        [HttpGet("/presentation/getPrevSlide"), Authorize]
        public ResponseMessage GetPrevSlide(string presentationId, string groupId)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            string presentationType = provider.Presentation.GetPresentationType(Guid.Parse(presentationId));
            bool isOwner = provider.Presentation.presentationExisted(Guid.Parse(presentationId), userId);
            if (presentationType == "public")
            {
                if (isOwner)
                {
                    if (!provider.Presentation.isPresentating(Guid.Parse(presentationId)))
                    {
                        return new ResponseMessage
                        {
                            status = false,
                            data = null,
                            message = "Presentation is not presented"
                        };
                    }
                    object prevSlide = provider.Presentation.GetPrevSlide(Guid.Parse(presentationId));
                    return new ResponseMessage
                    {
                        status = true,
                        data = prevSlide,
                        message = prevSlide != null ? "Get next slide successfully" : "Has meet start of presentation"
                    };
                }
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Only presentation onwner can get previous slide"
                };
            }
            if (presentationType == "group")
            {
                string userRole = provider.Group.GetMemberRoleInGroup(userId, Guid.Parse(groupId));
                bool isJoinedGroup = provider.Group.AlreadyJoinedGroup(userId, Guid.Parse(groupId));
                if (isJoinedGroup && (userRole == "Co-owner" || userRole == "Owner"))
                {
                    if (!provider.Presentation.isPresentating(Guid.Parse(presentationId)))
                    {
                        return new ResponseMessage
                        {
                            status = false,
                            data = null,
                            message = "Presentation is not presented"
                        };
                    }
                    object prevSlide = provider.Presentation.GetPrevSlide(Guid.Parse(presentationId));
                    return new ResponseMessage
                    {
                        status = true,
                        data = prevSlide,
                        message = prevSlide != null ? "Get next slide successfully" : "Has meet start of presentation"
                    };
                }
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Only presentation onwner or co-owner can get previous slide"
                };
            }
            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Invalid presentation type"
            };
        }

        [HttpGet("/presentation/present/info"), Authorize]
        public ResponseMessage GetPresentationInfoTeacher(string presentationId)
        {
            bool canParse = true;
            try
            {
                Guid.Parse(presentationId);
            }
            catch
            {
                canParse = false;

            }
            if (canParse is false)
            {
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Guid format is invalid"
                };
            }
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (provider.Presentation.isPresentating(Guid.Parse(presentationId)))
            {
                string presentationType = provider.Presentation.GetPresentationType(Guid.Parse(presentationId));
                if (presentationType == "public")
                {
                    bool isOwner = provider.Presentation.presentationExisted(Guid.Parse(presentationId), userId);
                    if (isOwner)
                    {

                        return new ResponseMessage
                        {
                            status = true,
                            data = new
                            {
                                presentationType = presentationType,
                                groupId = "null"
                            },
                            message = "Get presentation info successfully"
                        };
                    }
                }
                else if (presentationType == "group")
                {
                    Guid groupId = provider.Group.GetGroupByPresentationId(presentationId);
                    bool isTeacher = provider.Group.IsOwnerOrCoowner(userId, groupId);
                    if (isTeacher)
                    {
                        return new ResponseMessage
                        {
                            status = true,
                            data = new
                            {
                                presentationType = presentationType,
                                groupId = groupId
                            },
                            message = "Get presentation info successfully"
                        };
                    }
                }
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "You don't have permission to access"
                };
            }
            else
            {
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Presentation is not being presented"
                };
            }
        }

        [HttpGet("/presentation/group/view/info"), Authorize]
        public ResponseMessage GetGroupPresentationInfoStudent(string presentationId, string groupId)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            bool canParse = true;
            try
            {
                Guid.Parse(presentationId);
            } catch
            {
                canParse = false;

            }
            if (canParse is false)
            {
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Guid format is invalid"
                };
            }
            bool isMember = provider.Group.isMember(userId, Guid.Parse(groupId));
            if (isMember)
            {
                return new ResponseMessage
                {
                    status = true,
                    data = null,
                    message = "You have permission to access the presentation"
                };
            }
            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "You don't have permission to access"
            };
        }

        [HttpGet("/presentation/getType")]
        public ResponseMessage GetPresentationType(string presentationId)
        {
            bool canParse = true;
            try
            {
                Guid.Parse(presentationId);
            }
            catch
            {
                canParse = false;

            }
            if (canParse is false)
            {
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Guid format is invalid"
                };
            }
            bool isBeingPresented = provider.Presentation.isPresentating(Guid.Parse(presentationId));
            if (isBeingPresented)
            {
                string presentationType = provider.Presentation.GetPresentationType(Guid.Parse(presentationId));
                string groupId = presentationType == "group" ? provider.Group.GetGroupByPresentationId(presentationId).ToString() : "null";
                return new ResponseMessage
                {
                    status = true,
                    data = new
                    {
                        presentationType = presentationType,
                        groupId = groupId
                    },
                    message = "Get presentation type successfully"
                };
            }
            else
            {
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Presentation is not being presented"
                };
            }
        }

        [HttpGet("/presentation/get/numOfPresentations"), Authorize]
        public ResponseMessage GetNumofPresentations() {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            List<object> ownedPresentations = provider.Presentation.GetPresentationList(userId);
            List<object> collabPresentations = provider.Presentation.GetCollaboratorPresentation(userId);
            return new ResponseMessage
            {
                status = true,
                data = new
                {
                    ownedPresentations = ownedPresentations.Count,
                    collabPresentations = collabPresentations.Count
                },
                message = "Get number of presentations successfully"
            };
        }

        [HttpGet("/presentation/get/multipleChoiceQuestions"), Authorize]
        public ResponseMessage GetMultipleChoiceQuestions(string presentationId)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (provider.Presentation.GetPresentationType(Guid.Parse(presentationId)) == "group")
            {
                Guid presentingGroup = provider.Presentation.GetPresentGroup(Guid.Parse(presentationId));
                if (provider.Group.GetMemberRoleInGroup(userId, presentingGroup) == "Co-owner" || provider.Group.GetMemberRoleInGroup(userId, presentingGroup) == "Owner")
                {
                    return new ResponseMessage
                    {
                        status = true,
                        data = provider.MultipleChoiceSlide.GetMultipleChoiceQuestion(Guid.Parse(presentationId)),
                        message = "Get questions successfully"
                    };
                }
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Only owner and co-owner can get questions"
                };
            }
            if (provider.Presentation.GetPresentationType(Guid.Parse(presentationId)) == "public")
            {
                if (provider.Presentation.presentationExisted(Guid.Parse(presentationId), userId)) {
                    return new ResponseMessage
                    {
                        status = true,
                        data = provider.MultipleChoiceSlide.GetMultipleChoiceQuestion(Guid.Parse(presentationId)),
                        message = "Get questions successfully"
                    };
                }
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Only owner can get questions"
                };
            }
            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Invalide presentation type"
            };
        }

        [HttpGet("/presentation/get/choiceResult"), Authorize]
        public ResponseMessage GetChoiceResult(string presentationId, string questionId)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (provider.Presentation.GetPresentationType(Guid.Parse(presentationId)) == "group")
            {
                Guid presentingGroup = provider.Presentation.GetPresentGroup(Guid.Parse(presentationId));
                if (provider.Group.GetMemberRoleInGroup(userId, presentingGroup) == "Co-owner" || provider.Group.GetMemberRoleInGroup(userId, presentingGroup) == "Owner")
                {
                    return new ResponseMessage
                    {
                        status = true,
                        data = provider.UserSubmitChoice.GetChoiceResult(questionId),
                        message = "Get choice result successfully"
                    };
                }
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Only owner and co-owner can get result"
                };
            }
            if (provider.Presentation.GetPresentationType(Guid.Parse(presentationId)) == "public")
            {
                if (provider.Presentation.presentationExisted(Guid.Parse(presentationId), userId))
                {
                    return new ResponseMessage
                    {
                        status = true,
                        data = provider.UserSubmitChoice.GetChoiceResult(questionId),
                        message = "Get result successfully"
                    };
                }
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Only owner can get result"
                };
            }
            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Invalide presentation type"
            };
        }
    }
}
