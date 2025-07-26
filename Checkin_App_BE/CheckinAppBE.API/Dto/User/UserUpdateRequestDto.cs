namespace Dto.User
{
    public class UserUpdateRequestDto
    {
        public string? DisplayName { get; set; }
        public string? Email { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public List<Guid>? RoleIds { get; set; }
    }
}
