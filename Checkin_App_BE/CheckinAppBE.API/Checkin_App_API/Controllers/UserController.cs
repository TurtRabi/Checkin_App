using Microsoft.AspNetCore.Mvc;
using Service.UserService;
using Dto.User;
using Microsoft.AspNetCore.Authorization;
using Common;
using System.Security.Claims;

namespace Checkin_App_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] // Only Admin can access User management APIs
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
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
        public async Task<ActionResult<ServiceResult<UserResponseDto>>> GetUserById(Guid userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user.IsSuccess)
            {
                return Ok(user);
            }
            return StatusCode(user.StatusCode, user);
        }

        [HttpPut("{userId}")]
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
        public async Task<ActionResult<ServiceResult>> DeleteUser(Guid userId)
        {
            var result = await _userService.DeleteUserAsync(userId);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("{userId}/change-password")]
        public async Task<ActionResult<ServiceResult>> AdminChangeUserPassword(Guid userId, [FromBody] AdminChangeUserPasswordRequestDto request)
        {
            var result = await _userService.AdminChangeUserPasswordAsync(userId, request);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("{userId}/assign-roles")]
        public async Task<ActionResult<ServiceResult>> AssignRolesToUser(Guid userId, [FromBody] List<Guid> roleIds)
        {
            var result = await _userService.AssignRolesToUserAsync(userId, roleIds);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return StatusCode(result.StatusCode, result);
        }

        // Endpoint for user to change their own password
        [Authorize]
        [HttpPost("me/change-password")]
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
    }
}