using System;

namespace Dto.StressLog
{
    public class StressLogResponseDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int StressLevel { get; set; }
        public DateTime LogTime { get; set; }
        public string? Notes { get; set; }
    }
}