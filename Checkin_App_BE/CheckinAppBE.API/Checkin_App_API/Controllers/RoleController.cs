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
        public async Task<ActionResult<ServiceResult<IEnumerable<RoleResponseDto>>>> GetAllRoles([FromQuery] RoleFilterRequestDto filter)
        {
            var roles = await _roleService.GetAllRolesAsync(filter);
            if (roles.IsSuccess)
            {
                return Ok(roles);
            }
            return StatusCode(roles.StatusCode, roles);
        }

        [HttpGet("{roleId}")]
        public async Task<ActionResult<ServiceResult<RoleResponseDto>>> GetRoleById(Guid roleId)
        {
            var role = await _roleService.GetRoleByIdAsync(roleId);
            if (role.IsSuccess)
            {
                return Ok(role);
            }
            return StatusCode(role.StatusCode, role);
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResult>> CreateRole([FromBody] RoleCreateRequestDto request)
        {
            var result = await _roleService.CreateRoleAsync(request);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{roleId}")]
        public async Task<ActionResult<ServiceResult>> UpdateRole(Guid roleId, [FromBody] RoleUpdateRequestDto request)
        {
            var result = await _roleService.UpdateRoleAsync(roleId, request);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{roleId}")]
        public async Task<ActionResult<ServiceResult>> DeleteRole(Guid roleId)
        {
            var result = await _roleService.DeleteRoleAsync(roleId);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return StatusCode(result.StatusCode, result);
        }
    }
}