using System;
using System.ComponentModel.DataAnnotations;

namespace Dto.StressLog
{
    public class StressLogCreateRequestDto
    {
        [Required]
        [Range(1, 10, ErrorMessage = "StressLevel must be between 1 and 10.")]
        public int StressLevel { get; set; }
        public string? Notes { get; set; }
    }
}