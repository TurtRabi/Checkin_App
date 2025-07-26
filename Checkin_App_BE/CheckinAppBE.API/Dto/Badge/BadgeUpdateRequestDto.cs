using System;
using System.ComponentModel.DataAnnotations;

namespace Dto.Badge
{
    public class BadgeUpdateRequestDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        [Url]
        [MaxLength(200)]
        public string? ImageUrl { get; set; }

        public int RequiredCheckins { get; set; }
        public int PointsAwarded { get; set; }
    }
}