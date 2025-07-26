namespace Dto.Authenticate.Response
{
    public class LoginResponseDto
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
        public required Dto.User.UserResponseDto User { get; set; }
    }
}
