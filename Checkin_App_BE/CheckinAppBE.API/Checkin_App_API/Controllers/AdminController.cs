using Microsoft.AspNetCore.Mvc;
using Service.AdminService;
using System;
using System.Threading.Tasks;

namespace Checkin_App_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpPost("user/{userId}/ban")]
        public async Task<IActionResult> BanUser(Guid userId)
        {
            await _adminService.BanUserAsync(userId);
            return Ok();
        }

        [HttpPost("user/{userId}/unban")]
        public async Task<IActionResult> UnbanUser(Guid userId)
        {
            await _adminService.UnbanUserAsync(userId);
            return Ok();
        }

        [HttpPost("user/{userId}/assign-role")]
        public async Task<IActionResult> AssignRole(Guid userId, [FromBody] string roleName)
        {
            await _adminService.AssignRoleAsync(userId, roleName);
            return Ok();
        }

        [HttpPost("landmark/{landmarkId}/approve")]
        public async Task<IActionResult> ApproveLandmark(Guid landmarkId)
        {
            await _adminService.ApproveLandmarkAsync(landmarkId);
            return Ok();
        }

        [HttpPost("landmark/{landmarkId}/reject")]
        public async Task<IActionResult> RejectLandmark(Guid landmarkId)
        {
            await _adminService.RejectLandmarkAsync(landmarkId);
            return Ok();
        }
    }
}