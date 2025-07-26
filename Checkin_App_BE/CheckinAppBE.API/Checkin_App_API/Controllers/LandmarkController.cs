using Common;
using Dto.Landmark;
using Microsoft.AspNetCore.Mvc;
using Service.LandmarkService;

namespace Checkin_App_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LandmarkController : ControllerBase
    {
        private readonly ILandmarkService _landmarkService;

        public LandmarkController(ILandmarkService landmarkService)
        {
            _landmarkService = landmarkService;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResult<IEnumerable<LandmarkResponseDto>>>> GetAllLandmarks()
        {
            var result = await _landmarkService.GetAllLandmarksAsync();
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResult<LandmarkResponseDto>>> GetLandmarkById(Guid id)
        {
            var result = await _landmarkService.GetLandmarkByIdAsync(id);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResult<LandmarkResponseDto>>> CreateLandmark([FromBody] LandmarkCreateRequestDto request)
        {
            var result = await _landmarkService.CreateLandmarkAsync(request);
            if (result.IsSuccess)
            {
                return StatusCode(StatusCodes.Status201Created, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ServiceResult<LandmarkResponseDto>>> UpdateLandmark(Guid id, [FromBody] LandmarkUpdateRequestDto request)
        {
            if (id != request.Id)
            {
                return BadRequest(ServiceResult<LandmarkResponseDto>.Fail("ID mismatch.", StatusCodes.Status400BadRequest));
            }
            var result = await _landmarkService.UpdateLandmarkAsync(request);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResult<bool>>> DeleteLandmark(Guid id)
        {
            var result = await _landmarkService.DeleteLandmarkAsync(id);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return StatusCode(result.StatusCode, result);
        }
    }
}