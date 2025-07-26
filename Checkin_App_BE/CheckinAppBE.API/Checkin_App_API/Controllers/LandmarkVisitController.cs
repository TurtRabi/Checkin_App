using Common;
using Dto.LandmarkVisit;
using Microsoft.AspNetCore.Mvc;
using Service.LandmarkVisitService;
using Service.TreasureService;
using System.Security.Claims;

namespace Checkin_App_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LandmarkVisitController : ControllerBase
    {
        private readonly ILandmarkVisitService _landmarkVisitService;
        private readonly ITreasureService _treasureService;

        public LandmarkVisitController(ILandmarkVisitService landmarkVisitService, ITreasureService treasureService)
        {
            _landmarkVisitService = landmarkVisitService;
            _treasureService = treasureService;
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResult<LandmarkVisitResponseDto>>> CheckInLandmark([FromBody] LandmarkVisitCreateRequestDto request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized(ServiceResult<LandmarkVisitResponseDto>.Fail("User not authenticated or invalid user ID.", StatusCodes.Status401Unauthorized));
            }

            var result = await _landmarkVisitService.CreateLandmarkVisitAsync(userId, request);
            if (result.IsSuccess)
            {
                // Attempt to open special treasure after successful check-in
                await _treasureService.OpenSpecialTreasureAsync(userId, result.Data.Id);

                return StatusCode(StatusCodes.Status201Created, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<ServiceResult<IEnumerable<LandmarkVisitResponseDto>>>> GetUserLandmarkVisits(Guid userId)
        {
            var result = await _landmarkVisitService.GetUserLandmarkVisitsAsync(userId);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return StatusCode(result.StatusCode, result);
        }
    }
}