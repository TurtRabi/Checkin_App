namespace Dto.Authenticate.Request
{
    public class OtpVerifyRequestDto
    {
        public string? Email { get; set; }
        public string? OtpCode { get; set; }
    }
}