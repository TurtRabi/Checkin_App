using System;

namespace Dto.StressLog
{
    public class StressLogFilterRequestDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Period { get; set; } // e.g., "week", "month"
    }
}