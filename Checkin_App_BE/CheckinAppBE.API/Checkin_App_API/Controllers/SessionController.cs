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
        public async Task<ActionResult<ServiceResult<IEnumerable<object>>>> GetMySessions()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return Unauthorized(ServiceResult<IEnumerable<object>>.Fail("User not authenticated or invalid user ID.", StatusCodes.Status401Unauthorized));
            }

            var sessions = await _authenticationService.GetUserSessionsAsync(userId);
            if (sessions.IsSuccess)
            {
                return Ok(sessions);
            }
            return StatusCode(sessions.StatusCode, sessions);
        }

        [HttpDelete("{sessionId}")]
        public async Task<ActionResult<ServiceResult>> RevokeSession(Guid sessionId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return Unauthorized(ServiceResult.Fail("User not authenticated or invalid user ID.", StatusCodes.Status401Unauthorized));
            }

            var result = await _authenticationService.RevokeSessionAsync(userId, sessionId);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("me/all-except-current")]
        public async Task<ActionResult<ServiceResult>> RevokeAllSessionsExceptCurrent()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return Unauthorized(ServiceResult.Fail("User not authenticated or invalid user ID.", StatusCodes.Status401Unauthorized));
            }

            var currentSessionIdClaim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti);
            if (currentSessionIdClaim == null || !Guid.TryParse(currentSessionIdClaim.Value, out Guid currentSessionId))
            {
                return Unauthorized(ServiceResult.Fail("Không thể xác định phiên hiện tại.", StatusCodes.Status401Unauthorized));
            }

            var result = await _authenticationService.RevokeAllSessionsExceptCurrentAsync(userId, currentSessionId);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return StatusCode(result.StatusCode, result);
        }
    }
}