
using Dto.RewardCard;
using Microsoft.AspNetCore.Mvc;
using Service.RewardCardService;
using Microsoft.AspNetCore.Authorization;

namespace Checkin_App_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RewardCardController : ControllerBase
    {
        private readonly IRewardCardService _rewardCardService;

        public RewardCardController(IRewardCardService rewardCardService)
        {
            _rewardCardService = rewardCardService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetRewardCards()
        {
            var result = await _rewardCardService.GetRewardCards();
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateRewardCard([FromBody] RewardCardResponseDto rewardCard)
        {
            var result = await _rewardCardService.CreateRewardCard(rewardCard);
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRewardCard(Guid id, [FromBody] RewardCardResponseDto rewardCard)
        {
            var result = await _rewardCardService.UpdateRewardCard(id, rewardCard);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRewardCard(Guid id)
        {
            var result = await _rewardCardService.DeleteRewardCard(id);
            return Ok(result);
        }
    }
}
