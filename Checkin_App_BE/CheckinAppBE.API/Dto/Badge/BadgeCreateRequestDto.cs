using System.ComponentModel.DataAnnotations;

namespace Dto.Badge
{
    public class BadgeCreateRequestDto
    {
        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        [Url]
        [MaxLength(200)]
        public string? ImageUrl { get; set; }

        public int RequiredCheckins { get; set; } = 0; // Default to 0 if not specified
        public int PointsAwarded { get; set; } = 0;
    }
}