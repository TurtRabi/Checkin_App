using Dto.Badge;
using Microsoft.AspNetCore.Mvc;
using Service.BadgeService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Checkin_App_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BadgeController : ControllerBase
    {
        private readonly IBadgeService _badgeService;

        public BadgeController(IBadgeService badgeService)
        {
            _badgeService = badgeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BadgeResponseDto>>> GetAllBadges()
        {
            var result = await _badgeService.GetAllBadgesAsync();
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BadgeResponseDto>> GetBadgeById(Guid id)
        {
            var result = await _badgeService.GetBadgeByIdAsync(id);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return NotFound(result.Message);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<UserBadgeResponseDto>>> GetUserBadges(Guid userId)
        {
            var result = await _badgeService.GetUserBadgesAsync(userId);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        // This endpoint might be used by an admin or internal process, not directly by a user
        [HttpPost("award")]
        public async Task<ActionResult<UserBadgeResponseDto>> AwardUserBadge(Guid userId, Guid badgeId)
        {
            var result = await _badgeService.AwardUserBadgeAsync(userId, badgeId);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        [HttpPost]
        public async Task<ActionResult<BadgeResponseDto>> CreateBadge([FromBody] BadgeCreateRequestDto badgeDto)
        {
            var result = await _badgeService.CreateBadgeAsync(badgeDto);
            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetBadgeById), new { id = result.Data.Id }, result.Data);
            }
            return BadRequest(result.Message);
        }

        [HttpPut]
        public async Task<ActionResult<BadgeResponseDto>> UpdateBadge([FromBody] BadgeUpdateRequestDto badgeDto)
        {
            var result = await _badgeService.UpdateBadgeAsync(badgeDto);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteBadge(Guid id)
        {
            var result = await _badgeService.DeleteBadgeAsync(id);
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