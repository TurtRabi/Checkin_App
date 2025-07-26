namespace Dto.Authenticate.Request
{
    public class LoginRequestDto
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
        public string? DeviceName { get; set; }
        public string? IpAddress { get; set; }
    }
}
