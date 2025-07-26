namespace Dto
{
    public class UserDto
    {
        public Guid UserId { get; set; }
        public required string DisplayName { get; set; }
        public required string Email { get; set; }
        public required string ProfilePictureUrl { get; set; }
    }
}
