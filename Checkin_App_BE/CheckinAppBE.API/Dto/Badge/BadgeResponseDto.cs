using System;
using System.Collections.Generic;

namespace Dto.Badge
{
    public class BadgeResponseDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int RequiredCheckins { get; set; }
        public int PointsAwarded { get; set; }
    }
}