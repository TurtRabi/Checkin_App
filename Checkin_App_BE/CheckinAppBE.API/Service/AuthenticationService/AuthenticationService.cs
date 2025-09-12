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
using Service.NotificationService; // Changed from Service.EmailService
using Dto;
using Service.UserService;
using Dto.Notification;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Checkin_App_API.Controllers; // Thêm dòng này

namespace Service.AuthenticationService
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IRedisService _redisService;
        private readonly IGoEmailClientService _goEmailClientService; // Changed from IEmailService
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticationService(IUnitOfWork unitOfWork, IConfiguration configuration, IRedisService redisService, IGoEmailClientService goEmailClientService, IUserService userService, IHttpContextAccessor httpContextAccessor) // Changed constructor
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _redisService = redisService;
            _goEmailClientService = goEmailClientService; // Changed assignment
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;

        }

        public async Task<ServiceResult<LoginResponseDto>> LoginAsync(LoginRequestDto request)
        {
            // 1. Tìm người dùng theo email
            var user = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(u => u.UserName == request.UserName);
            var userRoles = await _unitOfWork.UserRoleRepository.GetAllAsync();

            var roles1 = await _unitOfWork.RoleRepository.GetAllAsync();

            var joined = from ur in userRoles
                         join r in roles1 on ur.RoleId equals r.Id
                         where ur.UserId == user.Id
                         select new UserRole
                         {
                             UserId = ur.UserId,
                             RoleId = ur.RoleId,
                             Role = r
                         };

            user.UserRoles = joined.ToList();
            if (user == null)
            {
                return ServiceResult<LoginResponseDto>.Fail("UserName hoặc mật khẩu không đúng.", 400);
            }

            // 2. Kiểm tra mật khẩu
            var localAuth = await _unitOfWork.LocalAuthenticationRepository.GetFirstOrDefaultAsync(la => la.UserId == user.Id);
            if (localAuth == null || !BCrypt.Net.BCrypt.Verify(request.Password, localAuth.PasswordHash))
            {
                return ServiceResult<LoginResponseDto>.Fail("Email hoặc mật khẩu không đúng.", 400);
            }

            // 3. Tạo JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured.");
            var accessTokenExpirationMinutesString = _configuration["Jwt:AccessTokenExpirationMinutes"] ?? throw new InvalidOperationException("Jwt:AccessTokenExpirationMinutes not configured.");
            var refreshTokenExpirationDaysString = _configuration["Jwt:RefreshTokenExpirationDays"] ?? throw new InvalidOperationException("Jwt:RefreshTokenExpirationDays not configured.");

            var accessTokenExpirationMinutes = Convert.ToDouble(accessTokenExpirationMinutesString);
            var refreshTokenExpirationDays = Convert.ToDouble(refreshTokenExpirationDaysString);
            var ipAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
            // Tạo UserSession mới
            var newSession = new UserSession
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                RefreshToken = Guid.NewGuid().ToString(), // Tạo refresh token mới
                DeviceName = request.DeviceName,
                IpAddress = ipAddress,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenExpirationDays)
            };

            await _unitOfWork.UserSessionRepository.AddAsync(newSession);
            await _unitOfWork.CommitAsync(); // Lưu session trước khi tạo JWT để có SessionId

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, newSession.Id.ToString())
            };

            if (!string.IsNullOrEmpty(user.Email))
            {
                claims.Add(new Claim(ClaimTypes.Email, user.Email));
            }

            // Lấy tất cả roles của user
            var roles = user.UserRoles.Select(ur => ur.Role.RoleName).ToList();
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(accessTokenExpirationMinutes),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);

            // Lưu AccessToken vào Redis với key theo UserID và DeviceName
            var accessTokenRedisKey = $"auth:token:{user.Id}:{newSession.DeviceName}";
            await _redisService.SetAsync(accessTokenRedisKey, accessToken, TimeSpan.FromMinutes(accessTokenExpirationMinutes));

            // Lưu RefreshToken vào Redis với key theo UserID và DeviceName
            var refreshTokenRedisKey = $"auth:refresh:{user.Id}:{newSession.DeviceName}";
            await _redisService.SetAsync(refreshTokenRedisKey, newSession.RefreshToken, TimeSpan.FromDays(refreshTokenExpirationDays));

            // 5. Trả về LoginResponseDto
            var userResponseDto = await _userService.GetUserByIdAsync(user.Id);
            if (!userResponseDto.IsSuccess)
            {
                return ServiceResult<LoginResponseDto>.Fail("Không thể lấy thông tin người dùng sau khi đăng nhập.", 500);
            }

            return ServiceResult<LoginResponseDto>.Success(new LoginResponseDto
            {
                AuthToken = accessTokenRedisKey,
                AuthRefresh = refreshTokenRedisKey, // Trả về refresh token từ session
                User = userResponseDto.Data
            });
        }

        public async Task<ServiceResult<LoginResponseDto>> RegisterAsync(RegisterRequestDto request)
        {
            // 1. Kiểm tra xem người dùng đã tồn tại chưa
            var existingUser = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(u => u.UserName == request.Username);
            if (existingUser != null)
            {
                return ServiceResult<LoginResponseDto>.Fail("UserName đã tồn tại.", 409);
            }

            // 2. Hash mật khẩu
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // 3. Tạo người dùng mới
            var newUser = new User
            {
                Id = Guid.NewGuid(),
                DisplayName = request.DisplayName,
                UserName = request.Username,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // 4. Tạo thông tin xác thực cục bộ
            var newLocalAuth = new LocalAuthentication
            {
                UserId = newUser.Id,
                PasswordHash = hashedPassword
            };

            // 5. Lưu vào cơ sở dữ liệu thông qua UserService
            var createResult = await _userService.CreateUserAndLocalAuthAsync(newUser, newLocalAuth);
            if (!createResult.IsSuccess)
            {
                return ServiceResult<LoginResponseDto>.Fail(createResult.Message, createResult.StatusCode);
            }

            // 6. Gán vai trò "User" mặc định cho người dùng mới
            var defaultRole = await _unitOfWork.RoleRepository.GetFirstOrDefaultAsync(r => r.RoleName == "User");
            if (defaultRole == null)
            {
                return ServiceResult<LoginResponseDto>.Fail("Vai trò 'User' mặc định không tồn tại. Vui lòng tạo vai trò này trong hệ thống.", 500);
            }

            var newUserRole = new UserRole
            {
                UserId = newUser.Id,
                RoleId = defaultRole.Id
            };
            await _unitOfWork.UserRoleRepository.AddAsync(newUserRole);
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
                Id = Guid.NewGuid(),
                UserId = newUser.Id,
                RefreshToken = Guid.NewGuid().ToString(),
                DeviceName = "", // Có thể lấy từ request nếu có
                IpAddress = "",   // Có thể lấy từ request nếu có
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenExpirationDays)
            };

            await _unitOfWork.UserSessionRepository.AddAsync(newSession);
            await _unitOfWork.CommitAsync();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, newUser.Id.ToString()),
                    new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, newSession.Id.ToString())
                }.Union(newUser.Email != null ? new[] { new Claim(ClaimTypes.Email, newUser.Email) } : Enumerable.Empty<Claim>()).ToArray()),
                Expires = DateTime.UtcNow.AddMinutes(accessTokenExpirationMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);

            // Lưu AccessToken vào Redis với key theo UserID và DeviceName
            var accessTokenRedisKey = $"auth:token:{newUser.Id}:{newSession.DeviceName}";
            await _redisService.SetAsync(accessTokenRedisKey, accessToken, TimeSpan.FromMinutes(accessTokenExpirationMinutes));

            // Lưu RefreshToken vào Redis với key theo UserID và DeviceName
            var refreshTokenRedisKey = $"auth:refresh:{newUser.Id}:{newSession.DeviceName}";
            await _redisService.SetAsync(refreshTokenRedisKey, newSession.RefreshToken, TimeSpan.FromDays(refreshTokenExpirationDays));

            // 7. Trả về LoginResponseDto
            var userResponseDto = await _userService.GetUserByIdAsync(newUser.Id);
            if (!userResponseDto.IsSuccess)
            {
                return ServiceResult<LoginResponseDto>.Fail("Không thể lấy thông tin người dùng sau khi đăng ký.", 500);
            }

            return ServiceResult<LoginResponseDto>.Success(new LoginResponseDto
            {
                AuthToken = accessTokenRedisKey,
                AuthRefresh = refreshTokenRedisKey,
                User = userResponseDto.Data
            });
        }

        public async Task<ServiceResult<LoginResponseDto>> RefreshTokenAsync(string refreshToken)
        {
            // 1. Tìm UserSession bằng refresh token
            var existingSession = await _unitOfWork.UserSessionRepository.GetFirstOrDefaultAsync(s =>
                s.RefreshToken == refreshToken && !s.IsRevoked && s.ExpiresAt > DateTime.UtcNow);

            if (existingSession == null)
            {
                return ServiceResult<LoginResponseDto>.Fail("Refresh token không hợp lệ hoặc đã hết hạn.", 401);
            }

            // 2. Lấy thông tin người dùng
            var user = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(u => u.Id == existingSession.UserId);
            if (user == null)
            {
                return ServiceResult<LoginResponseDto>.Fail("Người dùng không tồn tại.", 404);
            }

            var userRoles = await _unitOfWork.UserRoleRepository.GetAllAsync();

            var roles1 = await _unitOfWork.RoleRepository.GetAllAsync();

            var joined = from ur in userRoles
                         join r in roles1 on ur.RoleId equals r.Id
                         where ur.UserId == user.Id
                         select new UserRole
                         {
                             UserId = ur.UserId,
                             RoleId = ur.RoleId,
                             Role = r
                         };

            user.UserRoles = joined.ToList();

            // 3. Thu hồi session cũ
            existingSession.IsRevoked = true;
            _unitOfWork.UserSessionRepository.Update(existingSession);

            // 4. Tạo session mới (Refresh Token Rotation)
            var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured.");
            var accessTokenExpirationMinutesString = _configuration["Jwt:AccessTokenExpirationMinutes"] ?? throw new InvalidOperationException("Jwt:AccessTokenExpirationMinutes not configured.");
            var refreshTokenExpirationDaysString = _configuration["Jwt:RefreshTokenExpirationDays"] ?? throw new InvalidOperationException("Jwt:RefreshTokenExpirationDays not configured.");

            var accessTokenExpirationMinutes = Convert.ToDouble(accessTokenExpirationMinutesString);
            var refreshTokenExpirationDays = Convert.ToDouble(refreshTokenExpirationDaysString);

            var newSession = new UserSession
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                RefreshToken = Guid.NewGuid().ToString(),
                DeviceName = existingSession.DeviceName, // Giữ nguyên thông tin thiết bị
                IpAddress = existingSession.IpAddress,   // Giữ nguyên thông tin IP
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenExpirationDays)
            };

            await _unitOfWork.UserSessionRepository.AddAsync(newSession);
            await _unitOfWork.CommitAsync(); // Lưu cả session cũ và mới

            // 5. Tạo cặp access token mới với SessionId của session mới
            var roles = user.UserRoles.Select(ur => ur.Role.RoleName).ToList();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, newSession.Id.ToString())
            };

            if (!string.IsNullOrEmpty(user.Email))
            {
                claims.Add(new Claim(ClaimTypes.Email, user.Email));
            }

            // ✅ Thêm role vào claim
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(accessTokenExpirationMinutes),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };


            

            var newAccessToken = tokenHandler.CreateToken(tokenDescriptor);
            var newAccessTokenString = tokenHandler.WriteToken(newAccessToken);

            // Lưu AccessToken mới vào Redis với key theo UserID và DeviceName
            var accessTokenRedisKey = $"auth:token:{user.Id}:{newSession.DeviceName}";
            await _redisService.SetAsync(accessTokenRedisKey, newAccessTokenString, TimeSpan.FromMinutes(accessTokenExpirationMinutes));

            // Lưu RefreshToken mới vào Redis với key theo UserID và DeviceName
            var refreshTokenRedisKey = $"auth:refresh:{user.Id}:{newSession.DeviceName}";
            await _redisService.SetAsync(refreshTokenRedisKey, newSession.RefreshToken, TimeSpan.FromDays(refreshTokenExpirationDays));

            // 6. Trả về LoginResponseDto mới
            var userResponseDto = await _userService.GetUserByIdAsync(user.Id);
            if (!userResponseDto.IsSuccess)
            {
                return ServiceResult<LoginResponseDto>.Fail("Không thể lấy thông tin người dùng sau khi làm mới token.", 500);
            }

            return ServiceResult<LoginResponseDto>.Success(new LoginResponseDto
            {
                AuthToken = accessTokenRedisKey,
                AuthRefresh = refreshTokenRedisKey,
                User = userResponseDto.Data
            });
        }

        public async Task<ServiceResult> SendOtpAsync(OtpSendRequestDto request)
        {
            if (string.IsNullOrEmpty(request.Email) || !IsValidEmail(request.Email))
            {
                return ServiceResult.Fail("Địa chỉ email không hợp lệ.", 400);
            }

            string otpCode = new Random().Next(100000, 999999).ToString();
            string key = $"otp:{request.Email}";
            TimeSpan expiration = TimeSpan.FromMinutes(5); // OTP hết hạn sau 5 phút

            await _redisService.SetAsync(key, otpCode, expiration);

            try
            {
                string subject = "Mã OTP của bạn";
                string message = $"Mã OTP của bạn là: {otpCode}";
                var emailRequest = new GoEmailRequestDto
                {
                    To = request.Email,
                    Subject = subject,
                    Body = message
                };
                var result = await _goEmailClientService.SendEmailToGOServiceAsync(emailRequest);
                if (result.IsSuccess)
                {
                    return ServiceResult.Success("Mã OTP đã được gửi đến email của bạn.");
                }
                return ServiceResult.Fail($"Đã xảy ra lỗi khi gửi email: {result.Message}", result.StatusCode);
            }
            catch (Exception ex)
            {
                // Log the exception (using a proper logging framework is recommended)
                Console.WriteLine($"Lỗi không xác định khi gửi OTP: {ex.Message}");
                return ServiceResult.Fail("Đã xảy ra lỗi không xác định khi gửi OTP.", 500);
            }
        }

        public async Task<ServiceResult> VerifyOtpAsync(OtpVerifyRequestDto request)
        {
            if (string.IsNullOrEmpty(request.Email) || !IsValidEmail(request.Email))
            {
                return ServiceResult.Fail("Địa chỉ email không hợp lệ.", 400);
            }
            if (string.IsNullOrEmpty(request.OtpCode))
            {
                return ServiceResult.Fail("Mã OTP không được để trống.", 400);
            }

            string key = $"otp:{request.Email}";
            string? storedOtp = await _redisService.GetAsync<string>(key);

            if (string.IsNullOrEmpty(storedOtp) || storedOtp != request.OtpCode)
            {
                return ServiceResult.Fail("Mã OTP không hợp lệ hoặc đã hết hạn.", 400);
            }

            await _redisService.RemoveAsync(key); // Xóa OTP sau khi xác minh thành công

            return ServiceResult.Success("Xác minh OTP thành công.");
        }

        public async Task<ServiceResult<LoginResponseDto>> SocialLoginAsync(SocialLoginRequestDto request)
        {
            if (string.IsNullOrEmpty(request.Provider) || string.IsNullOrEmpty(request.Token))
            {
                return ServiceResult<LoginResponseDto>.Fail("Thông tin đăng nhập mạng xã hội không hợp lệ.", 400);
            }

            if (!request.Provider.Equals("Google", StringComparison.OrdinalIgnoreCase))
            {
                return ServiceResult<LoginResponseDto>.Fail("Hiện tại chỉ hỗ trợ đăng nhập bằng Google.", 400);
            }

            string socialUserEmail;
            string socialUserId;
            string displayName;

            // ✅ Xác thực Google token
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _configuration["Authentication:Google:ClientId"] } // ClientId từ Google Console
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(request.Token, settings);
                socialUserEmail = payload.Email;
                socialUserId = payload.Subject; // unique id Google
                displayName = payload.Name ?? payload.Email;
            }
            catch
            {
                return ServiceResult<LoginResponseDto>.Fail("Token Google không hợp lệ.", 401);
            }

            // 1. Kiểm tra user đã tồn tại chưa
            var socialAuth = await _unitOfWork.SocialAuthenticationRepository.GetFirstOrDefaultAsync(sa =>
                sa.Provider == request.Provider && sa.ProviderKey == socialUserId);

            User? user;
            if (socialAuth == null)
            {
                // Người dùng mới
                user = new User
                {
                    Id = Guid.NewGuid(),
                    DisplayName = displayName,
                    UserName = socialUserEmail,
                    Email = socialUserEmail,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                socialAuth = new SocialAuthentication
                {
                    SocialAuthId = Guid.NewGuid(),
                    UserId = user.Id,
                    Provider = request.Provider,
                    ProviderKey = socialUserId,
                    CreatedAt = DateTime.UtcNow
                };

                var createResult = await _userService.CreateUserAndSocialAuthAsync(user, socialAuth);
                if (!createResult.IsSuccess)
                {
                    return ServiceResult<LoginResponseDto>.Fail(createResult.Message, createResult.StatusCode);
                }

                // Gán vai trò "User" mặc định cho người dùng mới
                var defaultRole = await _unitOfWork.RoleRepository.GetFirstOrDefaultAsync(r => r.RoleName == "User");
                if (defaultRole == null)
                {
                    // Or handle this case as a critical error, maybe the role should be seeded
                    return ServiceResult<LoginResponseDto>.Fail("Vai trò 'User' mặc định không tồn tại.", 500);
                }

                var newUserRole = new UserRole
                {
                    UserId = user.Id,
                    RoleId = defaultRole.Id
                };
                await _unitOfWork.UserRoleRepository.AddAsync(newUserRole);
                await _unitOfWork.CommitAsync(); // Commit the role assignment
            }
            else
            {
                // Người dùng đã tồn tại
                user = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(u => u.Id == socialAuth.UserId);



                var userRoles = await _unitOfWork.UserRoleRepository.GetAllAsync();

                var roles1 = await _unitOfWork.RoleRepository.GetAllAsync();

                var joined = from ur in userRoles
                             join r in roles1 on ur.RoleId equals r.Id
                             where ur.UserId == user.Id
                             select new UserRole
                             {
                                 UserId = ur.UserId,
                                 RoleId = ur.RoleId,
                                 Role = r   
                             };

                user.UserRoles = joined.ToList();
                if (user == null)
                {
                    return ServiceResult<LoginResponseDto>.Fail("Người dùng liên kết với Google không tồn tại.", 404);
                }
            }

            // 2. Tạo JWT & Refresh Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured.");
            var accessTokenExpirationMinutes = Convert.ToDouble(_configuration["Jwt:AccessTokenExpirationMinutes"]);
            var refreshTokenExpirationDays = Convert.ToDouble(_configuration["Jwt:RefreshTokenExpirationDays"]);
            var ipAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
            var newSession = new UserSession
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                RefreshToken = Guid.NewGuid().ToString(),
                DeviceName = request.Device, // optional
                IpAddress = ipAddress,  // optional
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenExpirationDays)
            };

            await _unitOfWork.UserSessionRepository.AddAsync(newSession);
            await _unitOfWork.CommitAsync();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, newSession.Id.ToString())
            };


            

            var roles = user.UserRoles.Select(ur => ur.Role.RoleName).ToList();
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(accessTokenExpirationMinutes),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };
            var accessToken = tokenHandler.CreateToken(tokenDescriptor);
            var accessTokenString = tokenHandler.WriteToken(accessToken);

            // Lưu vào Redis

            String save_redis_auth_token = $"auth:token:{user.Id}:{newSession.DeviceName}";
            String save_redis_auth_refresh = $"auth:refresh:{user.Id}:{newSession.DeviceName}";
            await _redisService.SetAsync(save_redis_auth_token, accessTokenString, TimeSpan.FromMinutes(accessTokenExpirationMinutes));
            await _redisService.SetAsync(save_redis_auth_refresh, newSession.RefreshToken, TimeSpan.FromDays(refreshTokenExpirationDays));

            // 3. Trả về LoginResponseDto
            var userResponseDto = await _userService.GetUserByIdAsync(user.Id);
            if (!userResponseDto.IsSuccess)
            {
                return ServiceResult<LoginResponseDto>.Fail("Không thể lấy thông tin người dùng sau khi đăng nhập Google.", 500);
            }

            return ServiceResult<LoginResponseDto>.Success(new LoginResponseDto
            {
                AuthToken = save_redis_auth_token,
                AuthRefresh = save_redis_auth_refresh,
                User = userResponseDto.Data
            });
        }

        public async Task<ServiceResult> LinkSocialAccountAsync(Guid userId, LinkSocialAccountRequestDto request)
        {
            if (string.IsNullOrEmpty(request.Provider) || string.IsNullOrEmpty(request.Token))
            {
                return ServiceResult.Fail("Thông tin liên kết tài khoản mạng xã hội không hợp lệ.", 400);
            }

            string socialUserId;
            string? socialEmail = null;

            if (request.Provider.Equals("Google", StringComparison.OrdinalIgnoreCase))
            {
                // Xác thực Google token
                var payload = await GoogleJsonWebSignature.ValidateAsync(request.Token);
                socialUserId = payload.Subject; // subject = Google user id
                socialEmail = payload.Email;
            }
            else if (request.Provider.Equals("Facebook", StringComparison.OrdinalIgnoreCase))
            {
                // Gọi API Facebook để verify token và lấy userId
                // socialUserId = ...
                return ServiceResult.Fail("Facebook chưa implement.", 501);
            }
            else
            {
                return ServiceResult.Fail("Provider không được hỗ trợ.", 400);
            }

            // Check đã tồn tại
            var existingSocialAuth = await _unitOfWork.SocialAuthenticationRepository
                .GetFirstOrDefaultAsync(sa => sa.Provider == request.Provider && sa.ProviderKey == socialUserId);

            if (existingSocialAuth != null)
            {
                if (existingSocialAuth.UserId == userId)
                    return ServiceResult.Fail("Tài khoản MXH này đã được liên kết với bạn.", 409);
                else
                    return ServiceResult.Fail("Tài khoản MXH này đã liên kết với user khác.", 409);
            }

            // Check user tồn tại
            var user = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return ServiceResult.Fail("Người dùng không tồn tại.", 404);
            }
            else
            {
                // Cập nhật email nếu chưa có
                if (string.IsNullOrEmpty(user.Email) && !string.IsNullOrEmpty(socialEmail))
                {
                    user.Email = socialEmail;
                    _unitOfWork.UserRepository.Update(user);
                }
            }

                // Thêm liên kết mới
                var newSocialAuth = new SocialAuthentication
                {
                    SocialAuthId = Guid.NewGuid(),
                    UserId = userId,
                    Provider = request.Provider,
                    ProviderKey = socialUserId,
                    CreatedAt = DateTime.UtcNow
                };

            await _unitOfWork.SocialAuthenticationRepository.AddAsync(newSocialAuth);
            await _unitOfWork.CommitAsync();

            return ServiceResult.Success("Liên kết thành công.");
        }

        public async Task<ServiceResult> UnlinkSocialAccountAsync(Guid userId, UnlinkSocialAccountRequestDto request)
        {
            if (string.IsNullOrEmpty(request.Provider))
            {
                return ServiceResult.Fail("Thông tin nhà cung cấp mạng xã hội không hợp lệ.", 400);
            }

            // Find the social authentication record for the user and provider
            var socialAuth = await _unitOfWork.SocialAuthenticationRepository.GetFirstOrDefaultAsync(sa =>
                sa.UserId == userId && sa.Provider == request.Provider);

            if (socialAuth == null)
            {
                return ServiceResult.Fail("Không tìm thấy liên kết tài khoản mạng xã hội này cho người dùng.", 404);
            }

            // Ensure the user has at least one way to log in after unlinking (e.g., local password or another social account)
            var localAuth = await _unitOfWork.LocalAuthenticationRepository.GetFirstOrDefaultAsync(la => la.UserId == userId);
            var otherSocialAuths = await _unitOfWork.SocialAuthenticationRepository.FindAsync(sa => sa.UserId == userId && sa.Provider != request.Provider);

            if (localAuth == null && !otherSocialAuths.Any())
            {
                return ServiceResult.Fail("Không thể hủy liên kết tài khoản mạng xã hội cuối cùng. Người dùng phải có ít nhất một phương thức đăng nhập.", 400);
            }

            _unitOfWork.SocialAuthenticationRepository.Delete(socialAuth);
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

        public async Task<ServiceResult<IEnumerable<UserSession>>> GetUserSessionsAsync(Guid userId)
        {
            var sessions = await _unitOfWork.UserSessionRepository.FindAsync(s => s.UserId == userId && !s.IsRevoked && s.ExpiresAt > DateTime.UtcNow);
            return ServiceResult<IEnumerable<UserSession>>.Success(sessions);
        }

        public async Task<ServiceResult> RevokeSessionAsync(Guid userId, Guid sessionId)
        {
            var session = await _unitOfWork.UserSessionRepository.GetFirstOrDefaultAsync(s => s.Id == sessionId && s.UserId == userId);
            if (session == null)
            {
                return ServiceResult.Fail("Phiên không tồn tại hoặc không thuộc về người dùng này.", 404);
            }

            session.IsRevoked = true;
            _unitOfWork.UserSessionRepository.Update(session);
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
            var sessionsToRevoke = await _unitOfWork.UserSessionRepository.FindAsync(s => s.UserId == userId && s.Id != currentSessionId && !s.IsRevoked);

            foreach (var session in sessionsToRevoke)
            {
                session.IsRevoked = true;
                _unitOfWork.UserSessionRepository.Update(session);

                // Xóa AccessToken và RefreshToken khỏi Redis
                var accessTokenRedisKey = $"auth:token:{userId}:{session.DeviceName}";
                var refreshTokenRedisKey = $"auth:refresh:{userId}:{session.DeviceName}";
                await _redisService.RemoveAsync(accessTokenRedisKey);
                await _redisService.RemoveAsync(refreshTokenRedisKey);
            }
            await _unitOfWork.CommitAsync();

            return ServiceResult.Success("Tất cả các phiên khác đã được thu hồi thành công.");
        }

        public async Task<ServiceResult> FogotPassworld(ForgotPassworldRequet requet)
        {
            var checkUserExist = await _unitOfWork.UserRepository.FindAsync(s => s.UserName.Equals(requet.UserName) && s.Email.Equals(requet.Email));
            if (checkUserExist.Count() == 0)
            {
                return ServiceResult.Fail("Không tồn tại User");
            }
            var sendOtpResult = await SendOtpAsync(new OtpSendRequestDto { Email = requet.Email });
            if (!sendOtpResult.IsSuccess)
            {
                return ServiceResult.Fail($"Gửi OTP thất bại: {sendOtpResult.Message}");
            }
            return ServiceResult.Success("OTP đã được gửi tới email của bạn.");
        }

        public async Task<ServiceResult> getCurrentSession(Guid userId, Guid currentSessionId)
        {
            var session = await _unitOfWork.UserSessionRepository.GetFirstOrDefaultAsync(s => s.Id == currentSessionId && s.UserId == userId);
            if (session == null)
            {
                return ServiceResult.Fail("Phiên không tồn tại hoặc không thuộc về người dùng này.", 404);
            }

            session.IsRevoked = true;
            _unitOfWork.UserSessionRepository.Update(session);
            await _unitOfWork.CommitAsync();

            // Xóa AccessToken và RefreshToken khỏi Redis
            var accessTokenRedisKey = $"auth:token:{userId}:{session.DeviceName}";
            var refreshTokenRedisKey = $"auth:refresh:{userId}:{session.DeviceName}";
            await _redisService.RemoveAsync(accessTokenRedisKey);
            await _redisService.RemoveAsync(refreshTokenRedisKey);

            return ServiceResult.Success("Phiên đã được thu hồi thành công.");
        }

    }
}