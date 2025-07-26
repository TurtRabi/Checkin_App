using Common;
using Dto.Treasure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.TreasureService;
using System.Security.Claims;

namespace Checkin_App_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TreasureController : ControllerBase
    {
        private readonly ITreasureService _treasureService;

        public TreasureController(ITreasureService treasureService)
        {
            _treasureService = treasureService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllTreasures()
        {
            var result = await _treasureService.GetAllTreasuresAsync();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTreasureById(Guid id)
        {
            var result = await _treasureService.GetTreasureByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTreasure([FromBody] TreasureCreateRequestDto treasureDto)
        {
            var result = await _treasureService.CreateTreasureAsync(treasureDto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTreasure(Guid id, [FromBody] TreasureUpdateRequestDto treasureDto)
        {
            var result = await _treasureService.UpdateTreasureAsync(id, treasureDto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTreasure(Guid id)
        {
            var result = await _treasureService.DeleteTreasureAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("open-daily")]
        [Authorize]
        public async Task<IActionResult> OpenDailyTreasure()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ServiceResult<OpenTreasureResponseDto>.Fail("User not authenticated."));
            }

            var result = await _treasureService.OpenDailyTreasureAsync(Guid.Parse(userId));
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("open-special")]
        [Authorize]
        public async Task<IActionResult> OpenSpecialTreasure([FromBody] OpenTreasureRequestDto requestDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ServiceResult<OpenTreasureResponseDto>.Fail("User not authenticated."));
            }

            if (!requestDto.VisitId.HasValue)
            {
                return BadRequest(ServiceResult<OpenTreasureResponseDto>.Fail("VisitId is required for special treasure."));
            }

            var result = await _treasureService.OpenSpecialTreasureAsync(Guid.Parse(userId), requestDto.VisitId.Value);
            return StatusCode(result.StatusCode, result);
        }
    }
}
