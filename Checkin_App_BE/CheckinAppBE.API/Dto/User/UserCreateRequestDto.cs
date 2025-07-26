namespace Dto.User
{
    public class UserCreateRequestDto
    {
        public required string DisplayName { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public List<Guid> RoleIds { get; set; } = new List<Guid>();
    }
}