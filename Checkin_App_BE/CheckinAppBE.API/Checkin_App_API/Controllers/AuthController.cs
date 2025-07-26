using Microsoft.AspNetCore.Mvc;
using Service.AuthenticationService;
using Dto.Authenticate.Request;
using Dto.Authenticate.Response;
using Common;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Checkin_App_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var response = await _authenticationService.LoginAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Trong môi trường thực tế, nên trả về lỗi cụ thể hơn và log lỗi
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            try
            {
                var response = await _authenticationService.RegisterAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            try
            {
                var response = await _authenticationService.RefreshTokenAsync(request.RefreshToken);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] OtpSendRequestDto request)
        {
            try
            {
                var result = await _authenticationService.SendOtpAsync(request);
                if (result.Succeeded)
                {
                    return Ok(new { message = result.Message });
                }
                return BadRequest(new { errorCode = result.ErrorCode, message = result.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] OtpVerifyRequestDto request)
        {
            try
            {
                var result = await _authenticationService.VerifyOtpAsync(request);
                if (result.Succeeded)
                {
                    return Ok(new { message = result.Message });
                }
                return BadRequest(new { errorCode = result.ErrorCode, message = result.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("social-login")]
        public async Task<IActionResult> SocialLogin([FromBody] SocialLoginRequestDto request)
        {
            try
            {
                var response = await _authenticationService.SocialLoginAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("link-social")]
        public async Task<IActionResult> LinkSocialAccount([FromBody] LinkSocialAccountRequestDto request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
                {
                    return Unauthorized(new { message = "Không thể xác định người dùng." });
                }

                var result = await _authenticationService.LinkSocialAccountAsync(userId, request);
                if (result.Succeeded)
                {
                    return Ok(new { message = result.Message });
                }
                return BadRequest(new { errorCode = result.ErrorCode, message = result.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("unlink-social")]
        public async Task<IActionResult> UnlinkSocialAccount([FromBody] UnlinkSocialAccountRequestDto request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
                {
                    return Unauthorized(new { message = "Không thể xác định người dùng." });
                }

                var result = await _authenticationService.UnlinkSocialAccountAsync(userId, request);
                if (result.Succeeded)
                {
                    return Ok(new { message = result.Message });
                }
                return BadRequest(new { errorCode = result.ErrorCode, message = result.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}