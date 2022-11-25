using Cahut_Backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cahut_Backend.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class TokenController : BaseController
    {
        [HttpPost]
        [Route("token/refresh")]
        public ResponseMessage Refresh(Token tokens)
        {
            Guid UserId = Guid.Parse(TokenServices.GetUserIdFromExpiredToken(tokens.AccessToken));
            if(UserId != null)
            {
                if(provider.Token.ValidateRefreshToken(UserId, tokens.RefreshToken) == true)
                {
                    User usr = provider.User.GetUserById(UserId);
                    ClaimsIdentity claims = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, usr.UserName),
                        new Claim(ClaimTypes.Email, usr.Email),
                        new Claim(ClaimTypes.NameIdentifier, usr.UserId.ToString()),
                    });
                    Token token = new Token
                    {
                        AccessToken = TokenServices.CreateToken(claims),
                        RefreshToken = TokenServices.CreateRefreshToken()
                    };
                    return new ResponseMessage
                    {
                        status = true,
                        data = token,
                        message = "Refresh token success"
                    };
                }
            }
            return new ResponseMessage
            {
                status = false,
                data = null,
                message = "Invalid tokens"
            };
        }
    }
}
