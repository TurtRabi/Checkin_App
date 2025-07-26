using Microsoft.AspNetCore.Mvc;
using Service.RoleService;
using Dto.Role;
using Microsoft.AspNetCore.Authorization;
using Common;

namespace Checkin_App_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoles([FromQuery] RoleFilterRequestDto filter)
        {
            var roles = await _roleService.GetAllRolesAsync(filter);
            return Ok(roles);
        }

        [HttpGet("{roleId}")]
        public async Task<IActionResult> GetRoleById(Guid roleId)
        {
            var role = await _roleService.GetRoleByIdAsync(roleId);
            if (role == null)
            {
                return NotFound(new { message = "Vai trò không tồn tại." });
            }
            return Ok(role);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] RoleCreateRequestDto request)
        {
            var result = await _roleService.CreateRoleAsync(request);
            if (result.Succeeded)
            {
                return Ok(new { message = result.Message });
            }
            return BadRequest(new { errorCode = result.ErrorCode, message = result.Message });
        }

        [HttpPut("{roleId}")]
        public async Task<IActionResult> UpdateRole(Guid roleId, [FromBody] RoleUpdateRequestDto request)
        {
            var result = await _roleService.UpdateRoleAsync(roleId, request);
            if (result.Succeeded)
            {
                return Ok(new { message = result.Message });
            }
            return BadRequest(new { errorCode = result.ErrorCode, message = result.Message });
        }

        [HttpDelete("{roleId}")]
        public async Task<IActionResult> DeleteRole(Guid roleId)
        {
            var result = await _roleService.DeleteRoleAsync(roleId);
            if (result.Succeeded)
            {
                return Ok(new { message = result.Message });
            }
            return BadRequest(new { errorCode = result.ErrorCode, message = result.Message });
        }
    }
}
