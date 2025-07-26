using System;
using System.ComponentModel.DataAnnotations;

namespace Dto.Mission
{
    public class MissionUpdateRequestDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Title { get; set; }
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        [Required]
        [MaxLength(200)]
        public required string CompletionCriteria { get; set; }

        public bool IsDailyMission { get; set; }
        public int TargetValue { get; set; }
        public int PointsAwarded { get; set; }
    }
}