using System.ComponentModel.DataAnnotations;

namespace Dto.Mission
{
    public class MissionCreateRequestDto
    {
        [Required]
        [MaxLength(100)]
        public required string Title { get; set; }
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        [Required]
        [MaxLength(200)]
        public required string CompletionCriteria { get; set; }

        public bool IsDailyMission { get; set; } = false;
        public int TargetValue { get; set; } = 0;
        public int PointsAwarded { get; set; } = 0;
    }
}