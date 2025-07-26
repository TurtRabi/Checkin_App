using System;

namespace Dto.Mission
{
    public class UserMissionResponseDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid MissionId { get; set; }
        public string? Status { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public MissionResponseDto? Mission { get; set; }
    }
}