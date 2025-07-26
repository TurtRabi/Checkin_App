using Common;
using Dto.User;
using Repository.UWO;
using Repository.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace Service.UserService
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserService> _logger;

        public UserService(IUnitOfWork unitOfWork, IConfiguration configuration, ILogger<UserService> logger)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<ServiceResult<IEnumerable<UserResponseDto>>> GetAllUsersAsync(UserFilterRequestDto filter)
        {
            var query = _unitOfWork.UserRepository.Query();

            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                query = query.Where(u =>
                    (u.DisplayName != null && u.DisplayName.Contains(filter.Keyword)) ||
                    (u.Email != null && u.Email.Contains(filter.Keyword)));
            }

            if (filter.RoleId.HasValue)
            {
                query = query.Where(u => u.UserRoles.Any(ur => ur.RoleId == filter.RoleId.Value));
            }

            var users = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var userResponseDtos = new List<UserResponseDto>();
            foreach (var user in users)
            {
                var roleNames = await _unitOfWork.UserRoleRepository
                    .Query()
                    .Where(ur => ur.UserId == user.Id)
                    .Join(_unitOfWork.RoleRepository.Query(),
                          ur => ur.RoleId,
                          r => r.Id,
                          (ur, r) => r.RoleName)
                    .ToListAsync();

                userResponseDtos.Add(new UserResponseDto
                {
                    UserId = user.Id,
                    DisplayName = user.DisplayName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    ProfilePictureUrl = user.ProfilePictureUrl,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt,
                    RoleNames = roleNames
                });
            }

            return ServiceResult<IEnumerable<UserResponseDto>>.Success(userResponseDtos);
        }

        public async Task<ServiceResult<UserResponseDto>> GetUserByIdAsync(Guid userId)
        {
            var user = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return ServiceResult<UserResponseDto>.Fail("Người dùng không tồn tại.", 404);
            }

            var roleNames = await _unitOfWork.UserRoleRepository
                .Query()
                .Where(ur => ur.UserId == user.Id)
                .Join(_unitOfWork.RoleRepository.Query(),
                      ur => ur.RoleId,
                      r => r.Id,
                      (ur, r) => r.RoleName)
                .ToListAsync();

            return ServiceResult<UserResponseDto>.Success(new UserResponseDto
            {
                UserId = user.Id,
                DisplayName = user.DisplayName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                ProfilePictureUrl = user.ProfilePictureUrl,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                RoleNames = roleNames
            });
        }

        public async Task<ServiceResult> UpdateUserAsync(Guid userId, UserUpdateRequestDto request)
        {
            var user = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return ServiceResult.Fail("Người dùng không tồn tại.", 404);
            }

            if (!string.IsNullOrEmpty(request.DisplayName))
            {
                user.DisplayName = request.DisplayName;
            }
            if (!string.IsNullOrEmpty(request.Email))
            {
                var existingUserWithEmail = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(u => u.Email == request.Email && u.Id != userId);
                if (existingUserWithEmail != null)
                {
                    return ServiceResult.Fail("Email đã được sử dụng bởi người dùng khác.", 409);
                }
                user.Email = request.Email;
            }
            if (!string.IsNullOrEmpty(request.ProfilePictureUrl))
            {
                user.ProfilePictureUrl = request.ProfilePictureUrl;
            }

            user.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.UserRepository.Update(user);

            if (request.RoleIds != null)
            {
                var existingUserRoles = await _unitOfWork.UserRoleRepository.FindAsync(ur => ur.UserId == userId);
                foreach (var userRole in existingUserRoles)
                {
                    _unitOfWork.UserRoleRepository.Delete(userRole);
                }

                foreach (var roleId in request.RoleIds)
                {
                    var roleExists = await _unitOfWork.RoleRepository.GetFirstOrDefaultAsync(r => r.Id == roleId) != null;
                    if (!roleExists)
                    {
                        return ServiceResult.Fail($"Vai trò với ID {roleId} không tồn tại.", 404);
                    }
                    await _unitOfWork.UserRoleRepository.AddAsync(new UserRole { UserId = userId, RoleId = roleId });
                }
            }

            await _unitOfWork.CommitAsync();
            return ServiceResult.Success("Thông tin người dùng đã được cập nhật thành công.");
        }

        public async Task<ServiceResult> DeleteUserAsync(Guid userId)
        {
            var user = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return ServiceResult.Fail("Người dùng không tồn tại.", 404);
            }

            var localAuth = await _unitOfWork.LocalAuthenticationRepository.GetFirstOrDefaultAsync(la => la.UserId == userId);
            if (localAuth != null)
            {
                _unitOfWork.LocalAuthenticationRepository.Delete(localAuth);
            }

            var socialAuths = await _unitOfWork.SocialAuthenticationRepository.FindAsync(sa => sa.UserId == userId);
            foreach (var socialAuth in socialAuths)
            {
                _unitOfWork.SocialAuthenticationRepository.Delete(socialAuth);
            }

            var userRoles = await _unitOfWork.UserRoleRepository.FindAsync(ur => ur.UserId == userId);
            foreach (var userRole in userRoles)
            {
                _unitOfWork.UserRoleRepository.Delete(userRole);
            }

            var userBadges = await _unitOfWork.UserBadgeRepository.FindAsync(ub => ub.UserId == userId);
            foreach (var userBadge in userBadges)
            {
                _unitOfWork.UserBadgeRepository.Delete(userBadge);
            }

            var userMissions = await _unitOfWork.UserMissionRepository.FindAsync(um => um.UserId == userId);
            foreach (var userMission in userMissions)
            {
                _unitOfWork.UserMissionRepository.Delete(userMission);
            }

            var userTreasures = await _unitOfWork.UserTreasureRepository.FindAsync(ut => ut.UserId == userId);
            foreach (var userTreasure in userTreasures)
            {
                _unitOfWork.UserTreasureRepository.Delete(userTreasure);
            }

            var landmarkVisits = await _unitOfWork.LandmarkVisitRepository.FindAsync(lv => lv.UserId == userId);
            foreach (var landmarkVisit in landmarkVisits)
            {
                _unitOfWork.LandmarkVisitRepository.Delete(landmarkVisit);
            }

            var stressLogs = await _unitOfWork.StressLogRepository.FindAsync(sl => sl.UserId == userId);
            foreach (var stressLog in stressLogs)
            {
                _unitOfWork.StressLogRepository.Delete(stressLog);
            }

            _unitOfWork.UserRepository.Delete(user);
            await _unitOfWork.CommitAsync();

            return ServiceResult.Success("Người dùng đã được xóa thành công.");
        }

        public async Task<ServiceResult> ChangeUserPasswordAsync(Guid userId, UserChangePasswordRequestDto request)
        {
            var localAuth = await _unitOfWork.LocalAuthenticationRepository.GetFirstOrDefaultAsync(la => la.UserId == userId);
            if (localAuth == null)
            {
                return ServiceResult.Fail("Người dùng không có mật khẩu cục bộ để thay đổi.", 404);
            }

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, localAuth.PasswordHash))
            {
                return ServiceResult.Fail("Mật khẩu hiện tại không đúng.", 400);
            }

            localAuth.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            _unitOfWork.LocalAuthenticationRepository.Update(localAuth);
            await _unitOfWork.CommitAsync();

            return ServiceResult.Success("Mật khẩu đã được thay đổi thành công.");
        }

        public async Task<ServiceResult> AdminChangeUserPasswordAsync(Guid userId, AdminChangeUserPasswordRequestDto request)
        {
            var localAuth = await _unitOfWork.LocalAuthenticationRepository.GetFirstOrDefaultAsync(la => la.UserId == userId);
            if (localAuth == null)
            {
                localAuth = new LocalAuthentication
                {
                    UserId = userId,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword)
                };
                await _unitOfWork.LocalAuthenticationRepository.AddAsync(localAuth);
            }
            else
            {
                localAuth.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                _unitOfWork.LocalAuthenticationRepository.Update(localAuth);
            }
            await _unitOfWork.CommitAsync();

            return ServiceResult.Success("Mật khẩu người dùng đã được thay đổi thành công bởi quản trị viên.");
        }

        public async Task<ServiceResult> AssignRolesToUserAsync(Guid userId, List<Guid> roleIds)
        {
            var user = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return ServiceResult.Fail("Người dùng không tồn tại.", 404);
            }

            var existingUserRoles = await _unitOfWork.UserRoleRepository.FindAsync(ur => ur.UserId == userId);
            foreach (var userRole in existingUserRoles)
            {
                _unitOfWork.UserRoleRepository.Delete(userRole);
            }

            foreach (var roleId in roleIds)
            {
                var roleExists = await _unitOfWork.RoleRepository.GetFirstOrDefaultAsync(r => r.Id == roleId) != null;
                if (!roleExists)
                {
                    return ServiceResult.Fail($"Vai trò với ID {roleId} không tồn tại.", 404);
                }
                await _unitOfWork.UserRoleRepository.AddAsync(new UserRole { UserId = userId, RoleId = roleId });
            }

            await _unitOfWork.CommitAsync();

            return ServiceResult.Success("Vai trò của người dùng đã được cập nhật thành công.");
        }

        public async Task<ServiceResult> CreateUserAndLocalAuthAsync(User newUser, LocalAuthentication newLocalAuth)
        {
            await _unitOfWork.UserRepository.AddAsync(newUser);
            await _unitOfWork.LocalAuthenticationRepository.AddAsync(newLocalAuth);
            await _unitOfWork.CommitAsync();
            return ServiceResult.Success("Người dùng và thông tin xác thực cục bộ đã được tạo thành công.");
        }

        public async Task<ServiceResult> CreateUserAndSocialAuthAsync(User newUser, SocialAuthentication newSocialAuth)
        {
            await _unitOfWork.UserRepository.AddAsync(newUser);
            await _unitOfWork.SocialAuthenticationRepository.AddAsync(newSocialAuth);
            await _unitOfWork.CommitAsync();
            return ServiceResult.Success("Người dùng và thông tin xác thực mạng xã hội đã được tạo thành công.");
        }

        public async Task<ServiceResult<int>> GetUserLevelAsync(Guid userId)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return ServiceResult<int>.Fail("Người dùng không tồn tại.", 404);
            }

            // Simple level calculation: 100 points per level
            var level = (int)Math.Floor(user.Points / 100.0) + 1;
            return ServiceResult<int>.Success(level);
        }
    }
}