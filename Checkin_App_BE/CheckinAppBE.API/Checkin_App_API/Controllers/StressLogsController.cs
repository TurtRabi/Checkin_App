using Common;
using Dto.StressLog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.StressLogService;
using System.Security.Claims;

namespace Checkin_App_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StressLogsController : ControllerBase
    {
        private readonly IStressLogService _stressLogService;

        public StressLogsController(IStressLogService stressLogService)
        {
            _stressLogService = stressLogService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateStressLog([FromBody] StressLogCreateRequestDto requestDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ServiceResult<StressLogResponseDto>.Fail("User not authenticated."));
            }

            var result = await _stressLogService.CreateStressLogAsync(Guid.Parse(userId), requestDto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserStressLogs([FromQuery] StressLogFilterRequestDto filter)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ServiceResult<IEnumerable<StressLogResponseDto>>.Fail("User not authenticated."));
            }

            var result = await _stressLogService.GetUserStressLogsAsync(Guid.Parse(userId), filter);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("average-by-period")]
        public async Task<IActionResult> GetAverageStressLevelByPeriod([FromQuery] StressLogFilterRequestDto filter)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ServiceResult<Dictionary<string, double>>.Fail("User not authenticated."));
            }

            var result = await _stressLogService.GetAverageStressLevelByPeriodAsync(Guid.Parse(userId), filter);
            return StatusCode(result.StatusCode, result);
        }
    }
}
