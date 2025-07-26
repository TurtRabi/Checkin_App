namespace Dto.Authenticate.Request
{
    public class LinkSocialAccountRequestDto
    {
        public string? Provider { get; set; } // e.g., "Google", "Facebook"
        public string? Token { get; set; } // Token from the social provider
    }
}