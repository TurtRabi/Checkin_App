namespace Dto.Authenticate.Response
{
    public class LoginResponseDto
    {
        public required string AuthToken { get; set; }
        public required string AuthRefresh { get; set; }
        public required Dto.User.UserResponseDto User { get; set; }
    }
}
