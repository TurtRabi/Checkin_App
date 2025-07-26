using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Common;
using Service.AuthenticationService;
using System.Linq;
using System.Threading.Tasks;

namespace Checkin_App_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SessionController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public SessionController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMySessions()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return Unauthorized(new { message = "Không thể xác định người dùng." });
            }

            var sessions = await _authenticationService.GetUserSessionsAsync(userId);
            return Ok(sessions);
        }

        [HttpDelete("{sessionId}")]
        public async Task<IActionResult> RevokeSession(Guid sessionId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return Unauthorized(new { message = "Không thể xác định người dùng." });
            }

            var result = await _authenticationService.RevokeSessionAsync(userId, sessionId);
            if (result.Succeeded)
            {
                return Ok(new { message = result.Message });
            }
            return BadRequest(new { errorCode = result.ErrorCode, message = result.Message });
        }

        [HttpDelete("me/all-except-current")]
        public async Task<IActionResult> RevokeAllSessionsExceptCurrent()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return Unauthorized(new { message = "Không thể xác định người dùng." });
            }

            var currentSessionIdClaim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti);
            if (currentSessionIdClaim == null || !Guid.TryParse(currentSessionIdClaim.Value, out Guid currentSessionId))
            {
                return Unauthorized(new { message = "Không thể xác định phiên hiện tại." });
            }

            var result = await _authenticationService.RevokeAllSessionsExceptCurrentAsync(userId, currentSessionId);
            if (result.Succeeded)
            {
                return Ok(new { message = result.Message });
            }
            return BadRequest(new { errorCode = result.ErrorCode, message = result.Message });
        }
    }
}
