namespace Dto.Authenticate.Request
{
    public class RegisterRequestDto
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string DisplayName { get; set; }
    }
}
