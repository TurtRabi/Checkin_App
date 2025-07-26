using Dto.Authenticate.Request;
using Dto.Authenticate.Response;
using Common;
using Repository.Models;

namespace Service.AuthenticationService
{
    public interface IAuthenticationService
    {
        Task<ServiceResult<LoginResponseDto>> LoginAsync(LoginRequestDto request);
        Task<ServiceResult<LoginResponseDto>> RegisterAsync(RegisterRequestDto request);
        Task<ServiceResult<LoginResponseDto>> RefreshTokenAsync(string refreshToken);
        Task<ServiceResult> SendOtpAsync(OtpSendRequestDto request);
        Task<ServiceResult> VerifyOtpAsync(OtpVerifyRequestDto request);
        Task<ServiceResult<LoginResponseDto>> SocialLoginAsync(SocialLoginRequestDto request);
        Task<ServiceResult> LinkSocialAccountAsync(Guid userId, LinkSocialAccountRequestDto request);
        Task<ServiceResult> UnlinkSocialAccountAsync(Guid userId, UnlinkSocialAccountRequestDto request);
        Task<ServiceResult<IEnumerable<UserSession>>> GetUserSessionsAsync(Guid userId);
        Task<ServiceResult> RevokeSessionAsync(Guid userId, Guid sessionId);
        Task<ServiceResult> RevokeAllSessionsExceptCurrentAsync(Guid userId, Guid currentSessionId);
    }
}