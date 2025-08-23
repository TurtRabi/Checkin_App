namespace Checkin_App_API.Controllers
{
    public class ChangeForgotPasswordRequestDto
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string NewPassword { get; set; }
    }
}