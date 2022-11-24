using Cahut_Backend.Models;
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
                return new ResponseMessage
                {
                    status = addResult > 0?true:false,
                    data = null,
                    message = addResult > 0 ? "Join group successfully" : "Join group failed"
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
                    data = gr.JoinGrString,
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

       

    }
}

