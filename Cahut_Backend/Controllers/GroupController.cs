﻿using Cahut_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("group/invite/{grName}/{email}"), Authorize]
        public ResponseMessage InviteThroughMail(string email, string grName)
        {
            Group gr = provider.Group.GetGroupByName(grName);
            if (gr != null)
            {
                string bodyMsg = "";
                bodyMsg += $"<h2>Bạn vừa nhận được lời mời tham gia group {gr.GroupName} trên Cahut" +
                    ", vui lòng click vào link sau và làm theo hướng dẫn để tham gia group</h2>";
                bodyMsg += $"<h3>https://localhost:44326/group/join/{gr.JoinGrString}</h3>";
                EmailMessage msg = new EmailMessage
                {
                    EmailTo = email,
                    Subject = "Thư mời tham gia nhóm học tập",
                    Content = bodyMsg
                };
                EmailSender sender = provider.Email.GetMailSender();
                string sendMailResult = Helper.SendEmails(sender, msg, _configuration);
                if (sendMailResult == "Send mail success")
                {
                    provider.Email.increaseMailSent(sender.usr);
                    return new ResponseMessage
                    {
                        status = true,
                        data = null,
                        message = "Gửi mail mời người dùng tham gia thành công"
                    };
                }
            }
            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Gửi mail thất bại, group không tồn tại hoặc email của người dùng không đúng"
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
                    data = $"https://localhost:44326/group/join/{gr.JoinGrString}",
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

        [HttpPost("group/set/role/{grName}/{userName}/{roleName}")]
        public ResponseMessage SetMemberRole(string grName, string userEmail, string roleName)
        {
            Guid usrId = provider.User.GetUserIdByUserEmail(userEmail);
            int result = provider.Group.SetMemberRole(usrId, grName, roleName);
            return new ResponseMessage
            {
                status = result > 0 ? true : false,
                data = null,
                message = result > 0 ? "Set role success" : "Failed to set role"
            };
        }

        [HttpPost("group/manage/kick/{grName}/{userName}")]
        public ResponseMessage KickMember(string grName, string userEmail)
        {
            Group gr = provider.Group.GetGroupByName(grName);
            Guid userId = provider.User.GetUserIdByUserEmail(userEmail);
            int deleteResult = provider.Group.DeleteMember(gr.GroupId, userId);
            int updateResult = 0;
            if(deleteResult > 0)
            {
                updateResult = provider.Group.HandleGroupNumOfMembers(gr.GroupId, "delete");
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
    }
}

