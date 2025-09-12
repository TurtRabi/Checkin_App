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
        public async Task<ActionResult<ServiceResult<LoginResponseDto>>> Login([FromBody] LoginRequestDto request)
        {
            var response = await _authenticationService.LoginAsync(request);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("register")]
        public async Task<ActionResult<ServiceResult<LoginResponseDto>>> Register([FromBody] RegisterRequestDto request)
        {
            var response = await _authenticationService.RegisterAsync(request);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<ServiceResult<LoginResponseDto>>> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            var response = await _authenticationService.RefreshTokenAsync(request.RefreshToken);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("send-otp")]
        public async Task<ActionResult<ServiceResult>> SendOtp([FromBody] OtpSendRequestDto request)
        {
            var result = await _authenticationService.SendOtpAsync(request);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("verify-otp")]
        [AllowAnonymous]
        public async Task<ActionResult<ServiceResult>> VerifyOtp([FromBody] OtpVerifyRequestDto request)
        {
            var result = await _authenticationService.VerifyOtpAsync(request);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("social-login")]
        [AllowAnonymous]
        public async Task<ActionResult<ServiceResult<LoginResponseDto>>> SocialLogin([FromBody] SocialLoginRequestDto request)
        {
            var response = await _authenticationService.SocialLoginAsync(request);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response);
        }
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<ActionResult<ServiceResult<bool>>> ForgotPassworld([FromBody] ForgotPassworldRequet request)
        {
            var response = await _authenticationService.FogotPassworld(request);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpPost("link-social")]
        public async Task<ActionResult<ServiceResult>> LinkSocialAccount([FromBody] LinkSocialAccountRequestDto request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return Unauthorized(ServiceResult.Fail("User not authenticated or invalid user ID.", StatusCodes.Status401Unauthorized));
            }

            var result = await _authenticationService.LinkSocialAccountAsync(userId, request);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpPost("unlink-social")]
        public async Task<ActionResult<ServiceResult>> UnlinkSocialAccount([FromBody] UnlinkSocialAccountRequestDto request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return Unauthorized(ServiceResult.Fail("User not authenticated or invalid user ID.", StatusCodes.Status401Unauthorized));
            }

            var result = await _authenticationService.UnlinkSocialAccountAsync(userId, request);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return StatusCode(result.StatusCode, result);
        }
       
    }
}