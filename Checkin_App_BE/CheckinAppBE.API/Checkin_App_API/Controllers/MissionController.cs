using Dto.Mission;
using Microsoft.AspNetCore.Mvc;
using Service.MissionService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Checkin_App_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MissionController : ControllerBase
    {
        private readonly IMissionService _missionService;

        public MissionController(IMissionService missionService)
        {
            _missionService = missionService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MissionResponseDto>>> GetAllMissions()
        {
            var result = await _missionService.GetAllMissionsAsync();
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MissionResponseDto>> GetMissionById(Guid id)
        {
            var result = await _missionService.GetMissionByIdAsync(id);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return NotFound(result.Message);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<UserMissionResponseDto>>> GetUserMissions(Guid userId)
        {
            var result = await _missionService.GetUserMissionsAsync(userId);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        [HttpPost("assign")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserMissionResponseDto>> AssignUserMission(Guid userId, Guid missionId)
        {
            var result = await _missionService.AssignUserMissionAsync(userId, missionId);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        [HttpPost("complete/{userMissionId}")]
        public async Task<ActionResult<UserMissionResponseDto>> CompleteUserMission(Guid userMissionId)
        {
            var result = await _missionService.CompleteUserMissionAsync(userMissionId);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MissionResponseDto>> CreateMission([FromBody] MissionCreateRequestDto missionDto)
        {
            var result = await _missionService.CreateMissionAsync(missionDto);
            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetMissionById), new { id = result.Data.Id }, result.Data);
            }
            return BadRequest(result.Message);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MissionResponseDto>> UpdateMission([FromBody] MissionUpdateRequestDto missionDto)
        {
            var result = await _missionService.UpdateMissionAsync(missionDto);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> DeleteMission(Guid id)
        {
            var result = await _missionService.DeleteMissionAsync(id);
            if (result.IsSuccess)
            {
                if (result.Data)
                {
                    return NoContent(); // 204 No Content
                }
                return NotFound(result.Message);
            }
            return BadRequest(result.Message);
        }
    }
}