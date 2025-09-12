using Microsoft.AspNetCore.Mvc;
using Service.UserService;
using Dto.User;
using Microsoft.AspNetCore.Authorization;
using Common;
using System.Security.Claims;
using Dto.RewardCard;
using Service.RewardCardService; // Add this using

namespace Checkin_App_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRewardCardService _rewardCardService;

        public UserController(IUserService userService, IRewardCardService rewardCardService)
        {
            _userService = userService;
            _rewardCardService = rewardCardService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ServiceResult<IEnumerable<UserResponseDto>>>> GetAllUsers([FromQuery] UserFilterRequestDto filter)
        {
            var users = await _userService.GetAllUsersAsync(filter);
            if (users.IsSuccess)
            {
                return Ok(users);
            }
            return StatusCode(users.StatusCode, users);
        }

        [HttpGet("{userId}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<ServiceResult<UserResponseDto>>> GetUserById(Guid userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user.IsSuccess)
            {
                return Ok(user);
            }
            return StatusCode(user.StatusCode, user);
        }

        [HttpGet("{userId}/reward-cards")]
        [Authorize]
        public async Task<ActionResult<ServiceResult<IEnumerable<UserRewardCardResponseDto>>>> GetUserRewardCards(Guid userId)
        {
            var result = await _rewardCardService.GetUserRewardCards(userId);
            return Ok(result);
        }

        [HttpPut("{userId}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<ServiceResult>> UpdateUser(Guid userId, [FromBody] UserUpdateRequestDto request)
        {
            var result = await _userService.UpdateUserAsync(userId, request);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ServiceResult>> DeleteUser(Guid userId)
        {
            var result = await _userService.DeleteUserAsync(userId);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("{userId}/assign-roles")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ServiceResult>> AssignRolesToUser(Guid userId, [FromBody] List<Guid> roleIds)
        {
            var result = await _userService.AssignRolesToUserAsync(userId, roleIds);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("me/change-password")]
        [Authorize]
        public async Task<ActionResult<ServiceResult>> ChangeMyPassword([FromBody] UserChangePasswordRequestDto request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return Unauthorized(ServiceResult.Fail("User not authenticated or invalid user ID.", StatusCodes.Status401Unauthorized));
            }

            var result = await _userService.ChangeUserPasswordAsync(userId, request);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<ServiceResult<UserResponseDto>>> GetMyProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return Unauthorized(ServiceResult<UserResponseDto>.Fail("User not authenticated or invalid user ID.", StatusCodes.Status401Unauthorized));
            }
            var user = await _userService.GetUserByIdAsync(userId);
            if (user.IsSuccess)
            {
                return Ok(user);
            }
            return StatusCode(user.StatusCode, user);
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<ActionResult<ServiceResult>> ChangeForgotPassword([FromBody] ChangeForgotPasswordRequestDto request)
        {
            var result = await _userService.ChangeForgotPasswordAsyc(request);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return StatusCode(result.StatusCode, result);
        }
    }
}