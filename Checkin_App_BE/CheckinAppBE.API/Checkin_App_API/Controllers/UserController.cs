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
        public async Task<IActionResult> GetAllUsers([FromQuery] UserFilterRequestDto filter)
        {
            var users = await _userService.GetAllUsersAsync(filter);
            return Ok(users);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "Người dùng không tồn tại." });
            }
            return Ok(user);
        }

        

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UserUpdateRequestDto request)
        {
            var result = await _userService.UpdateUserAsync(userId, request);
            if (result.Succeeded)
            {
                return Ok(new { message = result.Message });
            }
            return BadRequest(new { errorCode = result.ErrorCode, message = result.Message });
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            var result = await _userService.DeleteUserAsync(userId);
            if (result.Succeeded)
            {
                return Ok(new { message = result.Message });
            }
            return BadRequest(new { errorCode = result.ErrorCode, message = result.Message });
        }

        [HttpPost("{userId}/change-password")]
        public async Task<IActionResult> AdminChangeUserPassword(Guid userId, [FromBody] AdminChangeUserPasswordRequestDto request)
        {
            var result = await _userService.AdminChangeUserPasswordAsync(userId, request);
            if (result.Succeeded)
            {
                return Ok(new { message = result.Message });
            }
            return BadRequest(new { errorCode = result.ErrorCode, message = result.Message });
        }

        [HttpPost("{userId}/assign-roles")]
        public async Task<IActionResult> AssignRolesToUser(Guid userId, [FromBody] List<Guid> roleIds)
        {
            var result = await _userService.AssignRolesToUserAsync(userId, roleIds);
            if (result.Succeeded)
            {
                return Ok(new { message = result.Message });
            }
            return BadRequest(new { errorCode = result.ErrorCode, message = result.Message });
        }

        // Endpoint for user to change their own password
        [Authorize]
        [HttpPost("me/change-password")]
        public async Task<IActionResult> ChangeMyPassword([FromBody] UserChangePasswordRequestDto request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return Unauthorized(new { message = "Không thể xác định người dùng." });
            }

            var result = await _userService.ChangeUserPasswordAsync(userId, request);
            if (result.Succeeded)
            {
                return Ok(new { message = result.Message });
            }
            return BadRequest(new { errorCode = result.ErrorCode, message = result.Message });
        }
    }
}
