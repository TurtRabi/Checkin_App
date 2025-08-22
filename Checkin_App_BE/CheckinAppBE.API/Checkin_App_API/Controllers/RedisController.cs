using Microsoft.AspNetCore.Mvc;
using Service.Redis;
using System.Threading.Tasks;

namespace Checkin_App_API.Controllers
{
    /// <summary>
    /// DTO for setting a value in Redis.
    /// </summary>
    public record RedisSetValueRequestDto(string Key, string Value, TimeSpan expireInSeconds);

    [ApiController]
    [Route("api/[controller]")]
    public class RedisController : ControllerBase
    {
        private readonly IRedisService _redisService;

        public RedisController(IRedisService redisService)
        {
            _redisService = redisService;
        }

        /// <summary>
        /// Gets a value from Redis by key.
        /// </summary>
        /// <param name="key">The key to retrieve.</param>
        [HttpGet("{key}")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetValue(string key)
        {
            var value = await _redisService.GetAsync<string>(key);
            if (string.IsNullOrEmpty(value))
            {
                return NotFound($"No value found for key: {key}");
            }
            return Ok(value);
        }

        /// <summary>
        /// Sets a key-value pair in Redis.
        /// </summary>
        /// <param name="request">The key and value to set.</param>
        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> SetValue([FromBody] RedisSetValueRequestDto request)
        {
            var success = await _redisService.SetAsync(request.Key, request.Value,request.expireInSeconds);
            if (success)
            {
                return NoContent(); // Successfully set
            }
            return BadRequest("Failed to set value in Redis.");
        }

        /// <summary>
        /// Deletes a value from Redis by key.
        /// </summary>
        /// <param name="key">The key to delete.</param>
        [HttpDelete("{key}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> DeleteValue(string key)
        {
            var success = await _redisService.RemoveAsync(key);
            if (success)
            {
                return NoContent(); // Successfully deleted
            }
            return BadRequest($"Failed to delete key: {key}");
        }
    }
}
