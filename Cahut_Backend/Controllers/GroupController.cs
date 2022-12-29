using Cahut_Backend.Models;
using Cahut_Backend.SignalR.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using System.Security.Claims;

namespace Cahut_Backend.Controllers
{
    [ApiController]
    public class GroupController : BaseController
    {
        private IConfiguration _configuration;
        public GroupController(IConfiguration configuration) : base()
        {
            this._configuration = configuration;
        }

        [HttpPost("group/create/{grName}"), Authorize]
        public ResponseMessage CreateGroup(string grName)
        {
            if(provider.Group.CheckGroupNameExisted(grName) == true)
            {
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Can not create, group name already existed"
                };
            }
            string OwnerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int result = provider.Group.CreateGroup(Guid.Parse(OwnerId), grName);
            return new ResponseMessage
            {
                status = result > 0 ? true : false,
                data = null,
                message = result > 0 ? "Create group success" : "Failed to create group"
            };
        }

        [HttpGet("group/join/{inviteString}"), Authorize]
        public ResponseMessage JoinGroupByLink(string inviteString)
        {
            Guid UserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            Group gr = provider.Group.GetGroupWithInviteString(inviteString);

            if (gr != null)
            {
                if (provider.Group.AlreadyJoinedGroup(UserId, gr.GroupId) == true)
                {
                    return new ResponseMessage
                    {
                        status = false,
                        data = null,
                        message = "Join group failed, you already joined in this group"
                    };
                }
                int addResult = provider.Group.AddMember(gr.GroupId, UserId);
                int updateResult = 0;
                if(addResult > 0)
                {
                    updateResult = provider.Group.HandleGroupNumOfMembers(gr.GroupId, "add");
                }
                return new ResponseMessage
                {
                    status = updateResult > 0?true:false,
                    data = null,
                    message = updateResult > 0 ? "Join group successfully" : "Join group failed"
                };
            }
            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Join group failed, group not exsist"
            };
        }

        private bool SendInvitationMail(Group gr, string email)
        {
            string bodyMsg = "";
            bodyMsg += $"<h2>You have just received an invitation to join the group {gr.GroupName} on Cahut" +
                ", Please click on the following link to join the group</h2>";
            bodyMsg += $"<h3>{Helper.TestingLink}/group/join/{gr.JoinGrString}</h3>";

            EmailMessage msg = new EmailMessage
            {
                EmailTo = email,
                Subject = "Invitation to join the study group",
                Content = bodyMsg
            };
            EmailSender sender = provider.Email.GetMailSender();
            string sendMailResult = Helper.SendEmails(sender, msg, _configuration);
            if (sendMailResult == "Send mail success")
            {
                provider.Email.increaseMailSent(sender.usr);
                return true;
            }
            return false;
        }

        [HttpPost("group/invitemany"), Authorize]
        public ResponseMessage SendInvitationToArrayEmail(object emailArray)
        {
            List<string> emails = new List<string>();
            JObject objTemp = JObject.Parse(emailArray.ToString());
            JArray emailJarr = (JArray)objTemp["emailArray"];
            string groupName = (string)objTemp["groupName"];
            emails = emailJarr.ToObject<List<string>>();
            Group gr = provider.Group.GetGroupByName(groupName);
            if(gr != null)
            {
                Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                string userRole = provider.Group.GetMemberRoleInGroup(userId, gr.GroupId);
                if (userRole == "Member")
                {
                    return new ResponseMessage
                    {
                        status = false,
                        data = null,
                        message = "Send invitation through mail failed, only Owner or Co-owner can use this feature"
                    };
                }
                int sendSuccess = 0;
                foreach (string email in emails)
                {
                    bool sendMailResult = SendInvitationMail(gr, email);
                    if(sendMailResult == true)
                    {
                        sendSuccess++;
                    }
                }
                if(sendSuccess == emails.Count)
                {
                    return new ResponseMessage
                    {
                        status = true,
                        data = null,
                        message = "Send join group invitation by array of emails success"
                    };
                }
                else if(sendSuccess != emails.Count && sendSuccess > 0)
                {
                    return new ResponseMessage
                    {
                        status = true,
                        data = null,
                        message = "Send join group invitation to some email in the array were successed, but some mail were failed to send"
                    };
                }
                else
                {
                    return new ResponseMessage
                    {
                        status = true,
                        data = null,
                        message = "Send join group invitation by array of email failed"
                    };
                }

            }
            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Email sending failed, group does not exist."
            };

            return new ResponseMessage
            {
                status = false,
                data = emailArray,
                message = "Send invitation through mail failed, only Owner or Co-owner can use this feature"
            };
        }

        [HttpPost("group/invite/{grName}/{email}"), Authorize]
        public ResponseMessage InviteThroughMail(string grName, string email)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            Group gr = provider.Group.GetGroupByName(grName);
            if (gr != null)
            {
                string userRole = provider.Group.GetMemberRoleInGroup(userId, gr.GroupId);
                if(userRole == "Member")
                {
                    return new ResponseMessage
                    {
                        status = false,
                        data = null,
                        message = "Send invitation through mail failed, only Owner or Co-owner can use this feature"
                    };
                }
                bool sendMailResult = SendInvitationMail(gr, email);
                
                if (sendMailResult == true)
                {
                    return new ResponseMessage
                    {
                        status = true,
                        data = null,
                        message = "Send email to invite users to join successfully"
                    };
                }
            }
            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Email sending failed, group does not exist."
            };
        }

        [HttpGet("group/get/invitelink/{grName}"), Authorize]
        public ResponseMessage GetGroupInviteLink(string grName)
        {
            Group gr = provider.Group.GetGroupByName(grName);
            if (gr is not null)
            {
                return new ResponseMessage
                {
                    status = true,
                    data = $"{Helper.TestingLink}/group/join/{gr.JoinGrString}",
                    message = "Get group invite link success"
                };
            }
            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Group is not exsist"
            };
        }

        [HttpGet("group/set/role/{grName}/{userEmail}/{roleName}")]
        public ResponseMessage SetMemberRole(string grName, string userEmail, string roleName)
        {
            Guid setterId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            Group gr = provider.Group.GetGroupByName(grName);
            if(gr == null)
            {
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Set member role failed, group does not exist"
                };
            }
            string setterRole = provider.Group.GetMemberRoleInGroup(setterId, gr.GroupId);
            if(setterRole != "Owner")
            {
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Set role failed, only group Owner can set member role."
                };
            }
            Guid usrId = provider.User.GetUserIdByUserEmail(userEmail);
            int result = provider.Group.SetMemberRole(usrId, grName, roleName);
            return new ResponseMessage
            {
                status = result > 0 ? true : false,
                data = null,
                message = result > 0 ? "Set role success" : "Failed to set role"
            };
        }

        [HttpGet("group/manage/kick/{grName}/{userEmail}")]
        public ResponseMessage KickMember(string grName, string userEmail)
        {
            Group gr = provider.Group.GetGroupByName(grName);
            Guid userId = provider.User.GetUserIdByUserEmail(userEmail);
            Guid kickerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            string kickedMemberRole = provider.Group.GetMemberRoleInGroup(userId, gr.GroupId);
            string kickerRole = provider.Group.GetMemberRoleInGroup(kickerId, gr.GroupId);
            int updateResult = 0;
            if ((kickedMemberRole == "Co-owner" && kickerRole == "Co-owner") || kickerRole == "Member" || kickedMemberRole == "Owner")
            {
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "You do not have authority to kick this user"
                };
            }
            else
            {
                int deleteResult = provider.Group.DeleteMember(gr.GroupId, userId);
                if (deleteResult > 0)
                {
                    updateResult = provider.Group.HandleGroupNumOfMembers(gr.GroupId, "delete");
                }
            }
            return new ResponseMessage
            {
                status = updateResult > 0 ? true : false,
                data = null,
                message = updateResult > 0 ? "Kick member success" : "Failed to kick member"
            };
        }

        [HttpGet("group/getall/groupmembers/{grName}"), Authorize]
        public ResponseMessage GetAllGroupMembers(string grName)
        {
            Group group = provider.Group.GetGroupByName(grName);
            if (group is not null)
            {
                return new ResponseMessage
                {
                    status = true,
                    data = provider.Group.getAllGrMembers(group.GroupId),
                    message = "Get all group members success"
                };
            }
            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Group is not exsist"
            };
        }

        [HttpGet("group/get/joined"), Authorize]
        public ResponseMessage GetJoinedGroup()
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var res = provider.Group.GetJoinedGroup(userId);
            return new ResponseMessage
            {
                status = res is not null ? true : false,
                data = res,
                message = res is not null ? "Get joined group successfully" : "You have not joined any group yet"
            };
        }

        [HttpGet("group/get/managed"), Authorize]
        public ResponseMessage GetManagedGroup()
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var res = provider.Group.GetManagedGroup(userId);
            return new ResponseMessage
            {
                status = res is not null ? true : false,
                data = res,
                message = res is not null ? "Get managed group successfully" : "You have not joined any group yet"
            };
        }

        [HttpGet("group/get/numofgroup"), Authorize]
        public ResponseMessage CountGroupNumParticipated()
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            List<object> res = provider.Group.GetJoinedGroup(userId);
            List<object> res2 = provider.Group.GetManagedGroup(userId);
            List<object> collabPresentations = provider.Presentation.GetCollaboratorPresentation(userId);
            return new ResponseMessage
            {
                status = true,
                data = new
                {
                    CollabPresentation = collabPresentations.Count,
                    PresentationNumber = provider.Presentation.CountPresentationOwned(userId),
                    GroupsIsOwner = res2.Count(),
                    GroupsIsNotOwner = res.Count(),
                },
                message = "Get num of user groups success"
            };
        }

        [HttpPost("group/delete"), Authorize]
        public ResponseMessage DeleteGroup(string grName)
        {
            Group group = provider.Group.GetGroupByName(grName);
            if (group is not null)
            {
                Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                string role = provider.Group.GetMemberRoleInGroup(userId, group.GroupId);
                if(role == "Owner")
                {
                    int deleteResult = provider.Group.DeleteGroup(group.GroupId);
                    return new ResponseMessage
                    {
                        status = deleteResult > 0 ? true : false,
                        data = null,
                        message = deleteResult > 0 ? "Delete group success" : "Delete group failed"
                    };
                }
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "Delete group failed, you do not have athourize"
                };
            }
            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Delete group failed, group does not exist",
            };
        }

        [HttpGet("group/getPresentation"), Authorize]
        public ResponseMessage GetPresentationInGroup(string groupName)
        {
            Group group = provider.Group.GetGroupByName(groupName);
            if (group is not null)
            {
                Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                
                if (provider.Group.AlreadyJoinedGroup(userId, group.GroupId))
                {
                    string presentation = provider.Group.GetPresentationInGroup(group.GroupId);
                    string role = provider.Group.GetMemberRoleInGroup(userId, group.GroupId);

                    return new ResponseMessage
                    {
                        status = presentation is not null ? true : false,
                        data = presentation is not null ? new { role = role, presentationId = presentation } : null,
                        message = presentation is not null ?  "Get presentation in group successfully" : "Group dont have presentation now"
                    };
                }
                return new ResponseMessage
                {
                    status = false,
                    data = null,
                    message = "You are not in this group"
                };
            }
            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Get presentation failed, group does not exist",
            };
        }
    }
}

