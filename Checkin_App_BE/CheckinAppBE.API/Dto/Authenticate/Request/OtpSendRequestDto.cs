namespace Dto.Authenticate.Request
{
    public class OtpSendRequestDto
    {
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}