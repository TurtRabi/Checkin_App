using System;
using System.Collections.Generic;

namespace Dto.User
{
    public class UserResponseDto
    {
        public Guid UserId { get; set; }
        public required string DisplayName { get; set; }
        public required string Email { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<string> RoleNames { get; set; } = new List<string>();
    }
}