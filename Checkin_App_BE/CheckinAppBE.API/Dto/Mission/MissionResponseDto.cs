using System;

namespace Dto.Mission
{
    public class MissionResponseDto
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? CompletionCriteria { get; set; }
        public bool IsDailyMission { get; set; }
        public int TargetValue { get; set; }
        public int PointsAwarded { get; set; }
    }
}