using System;

namespace Dto.Role
{
    public class RoleResponseDto
    {
        public Guid RoleId { get; set; }
        public required string RoleName { get; set; }
    }
}