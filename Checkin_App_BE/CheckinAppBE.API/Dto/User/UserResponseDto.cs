using System;
using System.Collections.Generic;

namespace Dto.User
{
    public class UserResponseDto
    {
        public Guid UserId { get; set; }
        public string? DisplayName { get; set; }
        public string? Email { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int Coin { get; set; }
        public int ExperiencePoints { get; set; }
        public List<string> RoleNames { get; set; } = new List<string>();
    }
}