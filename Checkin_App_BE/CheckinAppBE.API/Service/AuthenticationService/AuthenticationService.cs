using Dto.Authenticate.Request;
using Dto.Authenticate.Response;
using Repository.UWO;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Common;
using Service.Redis;
using BCrypt.Net;
using Repository.Models;
using Service.EmailService;
using Common;
using Dto;
using Service.UserService;

namespace Service.AuthenticationService
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IRedisService _redisService;
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;

        public AuthenticationService(IUnitOfWork unitOfWork, IConfiguration configuration, IRedisService redisService, IEmailService emailService, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _redisService = redisService;
            _emailService = emailService;
            _userService = userService;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            // 1. Tìm người dùng theo email
            var user = await _unitOfWork.User.GetFirstOrDefaultAsync(u => u.UserName == request.UserName);
            if (user == null)
            {
                throw new Exception("UserName hoặc mật khẩu không đúng.");
            }

            // 2. Kiểm tra mật khẩu
            var localAuth = await _unitOfWork.LocalAuthentication.GetFirstOrDefaultAsync(la => la.UserId == user.UserId);
            if (localAuth == null || !BCrypt.Net.BCrypt.Verify(request.Password, localAuth.PasswordHash))
            {
                throw new Exception("Email hoặc mật khẩu không đúng.");
            }

            // 3. Tạo JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured.");
            var accessTokenExpirationMinutesString = _configuration["Jwt:AccessTokenExpirationMinutes"] ?? throw new InvalidOperationException("Jwt:AccessTokenExpirationMinutes not configured.");
            var refreshTokenExpirationDaysString = _configuration["Jwt:RefreshTokenExpirationDays"] ?? throw new InvalidOperationException("Jwt:RefreshTokenExpirationDays not configured.");

            var accessTokenExpirationMinutes = Convert.ToDouble(accessTokenExpirationMinutesString);
            var refreshTokenExpirationDays = Convert.ToDouble(refreshTokenExpirationDaysString);

            // Tạo UserSession mới
            var newSession = new UserSession
            {
                SessionId = Guid.NewGuid(),
                UserId = user.UserId,
                RefreshToken = Guid.NewGuid().ToString(), // Tạo refresh token mới
                DeviceName = request.DeviceName,
                IpAddress = request.IpAddress,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenExpirationDays)
            };

            await _unitOfWork.UserSession.AddAsync(newSession);
            await _unitOfWork.CommitAsync(); // Lưu session trước khi tạo JWT để có SessionId

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, newSession.SessionId.ToString())
                }.Union(user.Email != null ? new[] { new Claim(ClaimTypes.Email, user.Email) } : Enumerable.Empty<Claim>()).ToArray()),
                Expires = DateTime.UtcNow.AddMinutes(accessTokenExpirationMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);

            // Lưu AccessToken vào Redis với key theo UserID và DeviceName
            var accessTokenRedisKey = $"auth:token:{user.UserId}:{newSession.DeviceName}";
            await _redisService.SetAsync(accessTokenRedisKey, accessToken, TimeSpan.FromMinutes(accessTokenExpirationMinutes));

            // Lưu RefreshToken vào Redis với key theo UserID và DeviceName
            var refreshTokenRedisKey = $"auth:refresh:{user.UserId}:{newSession.DeviceName}";
            await _redisService.SetAsync(refreshTokenRedisKey, newSession.RefreshToken, TimeSpan.FromDays(refreshTokenExpirationDays));

            // 5. Trả về LoginResponseDto
            var userResponseDto = await _userService.GetUserByIdAsync(user.UserId);
            if (userResponseDto == null)
            {
                throw new Exception("Không thể lấy thông tin người dùng sau khi đăng nhập.");
            }

            return new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = newSession.RefreshToken, // Trả về refresh token từ session
                User = userResponseDto
            };
        }

        public async Task<LoginResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            // 1. Kiểm tra xem người dùng đã tồn tại chưa
            var existingUser = await _unitOfWork.User.GetFirstOrDefaultAsync(u => u.UserName == request.Username);
            if (existingUser != null)
            {
                throw new Exception("UserName đã tồn tại.");
            }

            // 2. Hash mật khẩu
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // 3. Tạo người dùng mới
            var newUser = new User
            {
                UserId = Guid.NewGuid(),
                DisplayName = request.DisplayName,
                UserName = request.Username,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // 4. Tạo thông tin xác thực cục bộ
            var newLocalAuth = new LocalAuthentication
            {
                UserId = newUser.UserId,
                PasswordHash = hashedPassword
            };

            // 5. Lưu vào cơ sở dữ liệu thông qua UserService
            var createResult = await _userService.CreateUserAndLocalAuthAsync(newUser, newLocalAuth);
            if (!createResult.Succeeded)
            {
                throw new Exception(createResult.Message); // Hoặc xử lý lỗi cụ thể hơn
            }

            // 6. Gán vai trò "User" mặc định cho người dùng mới
            var defaultRole = await _unitOfWork.Role.GetFirstOrDefaultAsync(r => r.RoleName == "User");
            if (defaultRole == null)
            {
                throw new Exception("Vai trò 'User' mặc định không tồn tại. Vui lòng tạo vai trò này trong hệ thống.");
            }

            var newUserRole = new UserRole
            {
                UserId = newUser.UserId,
                RoleId = defaultRole.RoleId
            };
            await _unitOfWork.UserRole.AddAsync(newUserRole);
            await _unitOfWork.CommitAsync(); // Commit để lưu UserRole

            // 7. Tạo JWT và Refresh Token (tương tự như LoginAsync)
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured.");
            var accessTokenExpirationMinutesString = _configuration["Jwt:AccessTokenExpirationMinutes"] ?? throw new InvalidOperationException("Jwt:AccessTokenExpirationMinutes not configured.");
            var refreshTokenExpirationDaysString = _configuration["Jwt:RefreshTokenExpirationDays"] ?? throw new InvalidOperationException("Jwt:RefreshTokenExpirationDays not configured.");

            var accessTokenExpirationMinutes = Convert.ToDouble(accessTokenExpirationMinutesString);
            var refreshTokenExpirationDays = Convert.ToDouble(refreshTokenExpirationDaysString);

            // Tạo UserSession mới cho đăng ký
            var newSession = new UserSession
            {
                SessionId = Guid.NewGuid(),
                UserId = newUser.UserId,
                RefreshToken = Guid.NewGuid().ToString(),
                DeviceName = "", // Có thể lấy từ request nếu có
                IpAddress = "",   // Có thể lấy từ request nếu có
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenExpirationDays)
            };

            await _unitOfWork.UserSession.AddAsync(newSession);
            await _unitOfWork.CommitAsync();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, newUser.UserId.ToString()),
                    new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, newSession.SessionId.ToString())
                }.Union(newUser.Email != null ? new[] { new Claim(ClaimTypes.Email, newUser.Email) } : Enumerable.Empty<Claim>()).ToArray()),
                Expires = DateTime.UtcNow.AddMinutes(accessTokenExpirationMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);

            // Lưu AccessToken vào Redis với key theo UserID và DeviceName
            var accessTokenRedisKey = $"auth:token:{newUser.UserId}:{newSession.DeviceName}";
            await _redisService.SetAsync(accessTokenRedisKey, accessToken, TimeSpan.FromMinutes(accessTokenExpirationMinutes));

            // Lưu RefreshToken vào Redis với key theo UserID và DeviceName
            var refreshTokenRedisKey = $"auth:refresh:{newUser.UserId}:{newSession.DeviceName}";
            await _redisService.SetAsync(refreshTokenRedisKey, newSession.RefreshToken, TimeSpan.FromDays(refreshTokenExpirationDays));

            // 7. Trả về LoginResponseDto
            var userResponseDto = await _userService.GetUserByIdAsync(newUser.UserId);
            if (userResponseDto == null)
            {
                throw new Exception("Không thể lấy thông tin người dùng sau khi đăng ký.");
            }

            return new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = newSession.RefreshToken,
                User = userResponseDto
            };
        }

        public async Task<LoginResponseDto> RefreshTokenAsync(string refreshToken)
        {
            // 1. Tìm UserSession bằng refresh token
            var existingSession = await _unitOfWork.UserSession.GetFirstOrDefaultAsync(s =>
                s.RefreshToken == refreshToken && !s.IsRevoked && s.ExpiresAt > DateTime.UtcNow);

            if (existingSession == null)
            {
                throw new Exception("Refresh token không hợp lệ hoặc đã hết hạn.");
            }

            // 2. Lấy thông tin người dùng
            var user = await _unitOfWork.User.GetFirstOrDefaultAsync(u => u.UserId == existingSession.UserId);
            if (user == null)
            {
                throw new Exception("Người dùng không tồn tại.");
            }

            // 3. Thu hồi session cũ
            existingSession.IsRevoked = true;
            _unitOfWork.UserSession.Update(existingSession);

            // 4. Tạo session mới (Refresh Token Rotation)
            var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured.");
            var accessTokenExpirationMinutesString = _configuration["Jwt:AccessTokenExpirationMinutes"] ?? throw new InvalidOperationException("Jwt:AccessTokenExpirationMinutes not configured.");
            var refreshTokenExpirationDaysString = _configuration["Jwt:RefreshTokenExpirationDays"] ?? throw new InvalidOperationException("Jwt:RefreshTokenExpirationDays not configured.");

            var accessTokenExpirationMinutes = Convert.ToDouble(accessTokenExpirationMinutesString);
            var refreshTokenExpirationDays = Convert.ToDouble(refreshTokenExpirationDaysString);

            var newSession = new UserSession
            {
                SessionId = Guid.NewGuid(),
                UserId = user.UserId,
                RefreshToken = Guid.NewGuid().ToString(),
                DeviceName = existingSession.DeviceName, // Giữ nguyên thông tin thiết bị
                IpAddress = existingSession.IpAddress,   // Giữ nguyên thông tin IP
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenExpirationDays)
            };

            await _unitOfWork.UserSession.AddAsync(newSession);
            await _unitOfWork.CommitAsync(); // Lưu cả session cũ và mới

            // 5. Tạo cặp access token mới với SessionId của session mới
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, newSession.SessionId.ToString()) // Thêm SessionId mới vào JWT
                }.Union(user.Email != null ? new[] { new Claim(ClaimTypes.Email, user.Email) } : Enumerable.Empty<Claim>()).ToArray()),
                Expires = DateTime.UtcNow.AddMinutes(accessTokenExpirationMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)), SecurityAlgorithms.HmacSha256Signature)
            };
            var newAccessToken = tokenHandler.CreateToken(tokenDescriptor);
            var newAccessTokenString = tokenHandler.WriteToken(newAccessToken);

            // Lưu AccessToken mới vào Redis với key theo UserID và DeviceName
            var accessTokenRedisKey = $"auth:token:{user.UserId}:{newSession.DeviceName}";
            await _redisService.SetAsync(accessTokenRedisKey, newAccessTokenString, TimeSpan.FromMinutes(accessTokenExpirationMinutes));

            // Lưu RefreshToken mới vào Redis với key theo UserID và DeviceName
            var refreshTokenRedisKey = $"auth:refresh:{user.UserId}:{newSession.DeviceName}";
            await _redisService.SetAsync(refreshTokenRedisKey, newSession.RefreshToken, TimeSpan.FromDays(refreshTokenExpirationDays));

            // 6. Trả về LoginResponseDto mới
            var userResponseDto = await _userService.GetUserByIdAsync(user.UserId);
            if (userResponseDto == null)
            {
                throw new Exception("Không thể lấy thông tin người dùng sau khi làm mới token.");
            }

            return new LoginResponseDto
            {
                AccessToken = newAccessTokenString,
                RefreshToken = newSession.RefreshToken,
                User = userResponseDto
            };
        }

        public async Task<ServiceResult> SendOtpAsync(OtpSendRequestDto request)
        {
            if (string.IsNullOrEmpty(request.Email) || !IsValidEmail(request.Email))
            {
                return ServiceResult.Failed("HB40001", "Địa chỉ email không hợp lệ.");
            }

            string otpCode = new Random().Next(100000, 999999).ToString();
            string key = $"otp:{request.Email}";
            TimeSpan expiration = TimeSpan.FromMinutes(5); // OTP hết hạn sau 5 phút

            await _redisService.SetAsync(key, otpCode, expiration);

            try
            {
                string subject = "Mã OTP của bạn";
                string message = $"Mã OTP của bạn là: {otpCode}";
                await _emailService.SendEmailAsync(request.Email, subject, message);
                return ServiceResult.Success("Mã OTP đã được gửi đến email của bạn.");
            }
            catch (Exception ex)
            {
                // Log the exception (using a proper logging framework is recommended)
                Console.WriteLine($"Lỗi gửi email: {ex.Message}");
                return ServiceResult.Failed("HB50001", "Đã xảy ra lỗi khi gửi email.");
            }
        }

        public async Task<ServiceResult> VerifyOtpAsync(OtpVerifyRequestDto request)
        {
            if (string.IsNullOrEmpty(request.Email) || !IsValidEmail(request.Email))
            {
                return ServiceResult.Failed("HB40001", "Địa chỉ email không hợp lệ.");
            }
            if (string.IsNullOrEmpty(request.OtpCode))
            {
                return ServiceResult.Failed("HB40002", "Mã OTP không được để trống.");
            }

            string key = $"otp:{request.Email}";
            string? storedOtp = await _redisService.GetAsync<string>(key);

            if (string.IsNullOrEmpty(storedOtp) || storedOtp != request.OtpCode)
            {
                return ServiceResult.Failed("HB40003", "Mã OTP không hợp lệ hoặc đã hết hạn.");
            }

            await _redisService.RemoveAsync(key); // Xóa OTP sau khi xác minh thành công

            return ServiceResult.Success("Xác minh OTP thành công.");
        }

        public async Task<LoginResponseDto> SocialLoginAsync(SocialLoginRequestDto request)
        {
            // This is a simplified implementation.
            // In a real application, you would validate the token with the social provider (e.g., Google, Facebook).
            // For example, for Google, you'd use GoogleJsonWebSignature.ValidateAsync(request.Token).

            if (string.IsNullOrEmpty(request.Provider) || string.IsNullOrEmpty(request.Token))
            {
                throw new Exception("Thông tin đăng nhập mạng xã hội không hợp lệ.");
            }

            // Giả định: Lấy thông tin người dùng từ token (trong thực tế cần gọi API của nhà cung cấp)
            // Ví dụ:
            // var socialUserEmail = "user@example.com"; // Lấy từ việc giải mã token
            // var socialUserId = "social_id_from_provider"; // Lấy từ việc giải mã token

            // Để đơn giản, tôi sẽ giả định một email và ID dựa trên provider và token
            string socialUserEmail = $"{request.Provider.ToLower()}_{request.Token.Substring(0, 5)}@social.com";
            string socialUserId = $"{request.Provider.ToLower()}_{request.Token}";
            string displayName = $"{request.Provider} User";

            // 1. Kiểm tra xem người dùng đã tồn tại thông qua SocialAuthentication chưa
            var socialAuth = await _unitOfWork.SocialAuthentication.GetFirstOrDefaultAsync(sa =>
                sa.Provider == request.Provider && sa.ProviderKey == socialUserId);

            User? user;
            if (socialAuth == null)
            {
                // Người dùng chưa tồn tại, tạo mới
                user = new User
                {
                    UserId = Guid.NewGuid(),
                    DisplayName = displayName,
                    UserName = socialUserEmail, // Có thể sử dụng email làm username
                    Email = socialUserEmail, // Có thể lấy email từ thông tin trả về của nhà cung cấp
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _unitOfWork.User.AddAsync(user);

                socialAuth = new SocialAuthentication
                {
                    SocialAuthId = Guid.NewGuid(),
                    UserId = user.UserId,
                    Provider = request.Provider,
                    ProviderKey = socialUserId,
                    CreatedAt = DateTime.UtcNow
                };
                // Sử dụng UserService để tạo người dùng và social auth
                var createResult = await _userService.CreateUserAndSocialAuthAsync(user, socialAuth);
                if (!createResult.Succeeded)
                {
                    throw new Exception(createResult.Message);
                }
            }
            else
            {
                // Người dùng đã tồn tại, lấy thông tin người dùng
                user = await _unitOfWork.User.GetFirstOrDefaultAsync(u => u.UserId == socialAuth.UserId);
                if (user == null)
                {
                    throw new Exception("Người dùng liên kết với tài khoản mạng xã hội không tồn tại.");
                }
            }

            // 2. Tạo JWT và Refresh Token (tương tự như LoginAsync)
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured.");
            var accessTokenExpirationMinutesString = _configuration["Jwt:AccessTokenExpirationMinutes"] ?? throw new InvalidOperationException("Jwt:AccessTokenExpirationMinutes not configured.");
            var refreshTokenExpirationDaysString = _configuration["Jwt:RefreshTokenExpirationDays"] ?? throw new InvalidOperationException("Jwt:RefreshTokenExpirationDays not configured.");

            var accessTokenExpirationMinutes = Convert.ToDouble(accessTokenExpirationMinutesString);
            var refreshTokenExpirationDays = Convert.ToDouble(refreshTokenExpirationDaysString);

            // Tạo UserSession mới cho Social Login
            var newSession = new UserSession
            {
                SessionId = Guid.NewGuid(),
                UserId = user.UserId,
                RefreshToken = Guid.NewGuid().ToString(),
                DeviceName = "", // Có thể lấy từ request nếu có
                IpAddress = "",   // Có thể lấy từ request nếu có
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenExpirationDays)
            };

            await _unitOfWork.UserSession.AddAsync(newSession);
            await _unitOfWork.CommitAsync();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, newSession.SessionId.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(accessTokenExpirationMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)), SecurityAlgorithms.HmacSha256Signature)
            };
            var accessToken = tokenHandler.CreateToken(tokenDescriptor);
            var accessTokenString = tokenHandler.WriteToken(accessToken);

            // Lưu AccessToken vào Redis với key theo UserID và DeviceName
            var accessTokenRedisKey = $"auth:token:{user.UserId}:{newSession.DeviceName}";
            await _redisService.SetAsync(accessTokenRedisKey, accessTokenString, TimeSpan.FromMinutes(accessTokenExpirationMinutes));

            // Lưu RefreshToken vào Redis với key theo UserID và DeviceName
            var refreshTokenRedisKey = $"auth:refresh:{user.UserId}:{newSession.DeviceName}";
            await _redisService.SetAsync(refreshTokenRedisKey, newSession.RefreshToken, TimeSpan.FromDays(refreshTokenExpirationDays));

            // 3. Trả về LoginResponseDto
            var userResponseDto = await _userService.GetUserByIdAsync(user.UserId);
            if (userResponseDto == null)
            {
                throw new Exception("Không thể lấy thông tin người dùng sau khi đăng nhập mạng xã hội.");
            }

            return new LoginResponseDto
            {
                AccessToken = accessTokenString,
                RefreshToken = newSession.RefreshToken,
                User = userResponseDto
            };
        }

        public async Task<ServiceResult> LinkSocialAccountAsync(Guid userId, LinkSocialAccountRequestDto request)
        {
            if (string.IsNullOrEmpty(request.Provider) || string.IsNullOrEmpty(request.Token))
            {
                return ServiceResult.Failed("HB40004", "Thông tin liên kết tài khoản mạng xã hội không hợp lệ.");
            }

            // In a real application, you would validate the token with the social provider.
            // For simplicity, we'll assume the token is valid and extract a socialUserId.
            string socialUserId = $"{request.Provider.ToLower()}_{request.Token}";

            // Check if this social account is already linked to any user
            var existingSocialAuth = await _unitOfWork.SocialAuthentication.GetFirstOrDefaultAsync(sa =>
                sa.Provider == request.Provider && sa.ProviderKey == socialUserId);

            if (existingSocialAuth != null)
            {
                if (existingSocialAuth.UserId == userId)
                {
                    return ServiceResult.Failed("HB40005", "Tài khoản mạng xã hội này đã được liên kết với tài khoản của bạn.");
                }
                else
                {
                    return ServiceResult.Failed("HB40006", "Tài khoản mạng xã hội này đã được liên kết với một tài khoản khác.");
                }
            }

            // Check if the user exists
            var user = await _unitOfWork.User.GetFirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return ServiceResult.Failed("HB40402", "Người dùng không tồn tại.");
            }

            // Link the social account
            var newSocialAuth = new SocialAuthentication
            {
                SocialAuthId = Guid.NewGuid(),
                UserId = userId,
                Provider = request.Provider,
                ProviderKey = socialUserId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.SocialAuthentication.AddAsync(newSocialAuth);
            await _unitOfWork.CommitAsync();

            return ServiceResult.Success("Liên kết tài khoản mạng xã hội thành công.");
        }

        public async Task<ServiceResult> UnlinkSocialAccountAsync(Guid userId, UnlinkSocialAccountRequestDto request)
        {
            if (string.IsNullOrEmpty(request.Provider))
            {
                return ServiceResult.Failed("HB40007", "Thông tin nhà cung cấp mạng xã hội không hợp lệ.");
            }

            // Find the social authentication record for the user and provider
            var socialAuth = await _unitOfWork.SocialAuthentication.GetFirstOrDefaultAsync(sa =>
                sa.UserId == userId && sa.Provider == request.Provider);

            if (socialAuth == null)
            {
                return ServiceResult.Failed("HB40403", "Không tìm thấy liên kết tài khoản mạng xã hội này cho người dùng.");
            }

            // Ensure the user has at least one way to log in after unlinking (e.g., local password or another social account)
            var localAuth = await _unitOfWork.LocalAuthentication.GetFirstOrDefaultAsync(la => la.UserId == userId);
            var otherSocialAuths = await _unitOfWork.SocialAuthentication.FindAsync(sa => sa.UserId == userId && sa.Provider != request.Provider);

            if (localAuth == null && !otherSocialAuths.Any())
            {
                return ServiceResult.Failed("HB40008", "Không thể hủy liên kết tài khoản mạng xã hội cuối cùng. Người dùng phải có ít nhất một phương thức đăng nhập.");
            }

            await _unitOfWork.SocialAuthentication.DeleteAsync(socialAuth);
            await _unitOfWork.CommitAsync();

            return ServiceResult.Success("Hủy liên kết tài khoản mạng xã hội thành công.");
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<UserSession>> GetUserSessionsAsync(Guid userId)
        {
            return await _unitOfWork.UserSession.FindAsync(s => s.UserId == userId && !s.IsRevoked && s.ExpiresAt > DateTime.UtcNow);
        }

        public async Task<ServiceResult> RevokeSessionAsync(Guid userId, Guid sessionId)
        {
            var session = await _unitOfWork.UserSession.GetFirstOrDefaultAsync(s => s.SessionId == sessionId && s.UserId == userId);
            if (session == null)
            {
                return ServiceResult.Failed("HB40404", "Phiên không tồn tại hoặc không thuộc về người dùng này.");
            }

            session.IsRevoked = true;
            _unitOfWork.UserSession.Update(session);
            await _unitOfWork.CommitAsync();

            // Xóa AccessToken và RefreshToken khỏi Redis
            var accessTokenRedisKey = $"auth:token:{userId}:{session.DeviceName}";
            var refreshTokenRedisKey = $"auth:refresh:{userId}:{session.DeviceName}";
            await _redisService.RemoveAsync(accessTokenRedisKey);
            await _redisService.RemoveAsync(refreshTokenRedisKey);

            return ServiceResult.Success("Phiên đã được thu hồi thành công.");
        }

        public async Task<ServiceResult> RevokeAllSessionsExceptCurrentAsync(Guid userId, Guid currentSessionId)
        {
            var sessionsToRevoke = await _unitOfWork.UserSession.FindAsync(s => s.UserId == userId && s.SessionId != currentSessionId && !s.IsRevoked);

            foreach (var session in sessionsToRevoke)
            {
                session.IsRevoked = true;
                _unitOfWork.UserSession.Update(session);

                // Xóa AccessToken và RefreshToken khỏi Redis
                var accessTokenRedisKey = $"auth:token:{userId}:{session.DeviceName}";
                var refreshTokenRedisKey = $"auth:refresh:{userId}:{session.DeviceName}";
                await _redisService.RemoveAsync(accessTokenRedisKey);
                await _redisService.RemoveAsync(refreshTokenRedisKey);
            }
            await _unitOfWork.CommitAsync();

            return ServiceResult.Success("Tất cả các phiên khác đã được thu hồi thành công.");
        }
    }
}