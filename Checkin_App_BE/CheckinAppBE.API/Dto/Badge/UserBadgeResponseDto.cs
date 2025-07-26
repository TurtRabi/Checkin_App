using System;

namespace Dto.Badge
{
    public class UserBadgeResponseDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid BadgeId { get; set; }
        public DateTime EarnedAt { get; set; }
        public BadgeResponseDto? Badge { get; set; }
    }
}