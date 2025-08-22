namespace Dto.Authenticate.Request
{
    public class SocialLoginRequestDto
    {
        public string? Provider { get; set; } // e.g., "Google", "Facebook"
        public string? Device { get; set; }
        public string? Token { get; set; } // Token from the social provider
    }
}