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

        public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync(UserFilterRequestDto filter)
        {
            var query = _unitOfWork.User.Query();

            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                query = query.Where(u =>
                    u.DisplayName.Contains(filter.Keyword) ||
                    u.Email.Contains(filter.Keyword));
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
                var roleNames = await _unitOfWork.UserRole
                    .Query()
                    .Where(ur => ur.UserId == user.UserId)
                    .Join(_unitOfWork.Role.Query(), // Join with Role table
                          ur => ur.RoleId,
                          r => r.RoleId,
                          (ur, r) => r.RoleName)
                    .ToListAsync();

                userResponseDtos.Add(new UserResponseDto
                {
                    UserId = user.UserId,
                    DisplayName = user.DisplayName,
                    Email = user.Email,
                    ProfilePictureUrl = user.ProfilePictureUrl,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt,
                    RoleNames = roleNames
                });
            }

            return userResponseDtos;
        }

        public async Task<UserResponseDto?> GetUserByIdAsync(Guid userId)
        {
            var user = await _unitOfWork.User.GetFirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return null; // Or throw a specific exception
            }

            var roleNames = await _unitOfWork.UserRole
                .Query()
                .Where(ur => ur.UserId == user.UserId)
                .Join(_unitOfWork.Role.Query(),
                      ur => ur.RoleId,
                      r => r.RoleId,
                      (ur, r) => r.RoleName)
                .ToListAsync();

            return new UserResponseDto
            {
                UserId = user.UserId,
                DisplayName = user.DisplayName,
                Email = user.Email,
                ProfilePictureUrl = user.ProfilePictureUrl,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                RoleNames = roleNames
            };
        }

        

        public async Task<ServiceResult> UpdateUserAsync(Guid userId, UserUpdateRequestDto request)
        {
            var user = await _unitOfWork.User.GetFirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return ServiceResult.Failed("HB40404", "Người dùng không tồn tại.");
            }

            if (!string.IsNullOrEmpty(request.DisplayName))
            {
                user.DisplayName = request.DisplayName;
            }
            if (!string.IsNullOrEmpty(request.Email))
            {
                // Check if new email already exists for another user
                var existingUserWithEmail = await _unitOfWork.User.GetFirstOrDefaultAsync(u => u.Email == request.Email && u.UserId != userId);
                if (existingUserWithEmail != null)
                {
                    return ServiceResult.Failed("HB40012", "Email đã được sử dụng bởi người dùng khác.");
                }
                user.Email = request.Email;
            }
            if (!string.IsNullOrEmpty(request.ProfilePictureUrl))
            {
                user.ProfilePictureUrl = request.ProfilePictureUrl;
            }

            user.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.User.Update(user);

            // Update roles
            if (request.RoleIds != null)
            {
                // Remove existing roles for the user
                var existingUserRoles = await _unitOfWork.UserRole.FindAsync(ur => ur.UserId == userId);
                foreach (var userRole in existingUserRoles)
                {
                    await _unitOfWork.UserRole.DeleteAsync(userRole);
                }

                // Add new roles
                foreach (var roleId in request.RoleIds)
                {
                    var roleExists = await _unitOfWork.Role.GetFirstOrDefaultAsync(r => r.RoleId == roleId) != null;
                    if (!roleExists)
                    {
                        return ServiceResult.Failed("HB40013", $"Vai trò với ID {roleId} không tồn tại.");
                    }
                    await _unitOfWork.UserRole.AddAsync(new UserRole { UserId = userId, RoleId = roleId });
                }
            }

            await _unitOfWork.CommitAsync();

            return ServiceResult.Success("Thông tin người dùng đã được cập nhật thành công.");
        }

        public async Task<ServiceResult> DeleteUserAsync(Guid userId)
        {
            var user = await _unitOfWork.User.GetFirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return ServiceResult.Failed("HB40405", "Người dùng không tồn tại.");
            }

            // Remove associated local authentication
            var localAuth = await _unitOfWork.LocalAuthentication.GetFirstOrDefaultAsync(la => la.UserId == userId);
            if (localAuth != null)
            {
                await _unitOfWork.LocalAuthentication.DeleteAsync(localAuth);
            }

            // Remove associated social authentications
            var socialAuths = await _unitOfWork.SocialAuthentication.FindAsync(sa => sa.UserId == userId);
            foreach (var socialAuth in socialAuths)
            {
                await _unitOfWork.SocialAuthentication.DeleteAsync(socialAuth);
            }

            // Remove associated user roles
            var userRoles = await _unitOfWork.UserRole.FindAsync(ur => ur.UserId == userId);
            foreach (var userRole in userRoles)
            {
                await _unitOfWork.UserRole.DeleteAsync(userRole);
            }

            // Remove associated user badges, missions, treasures, landmark visits, stress logs
            // (Assuming cascade delete is not configured or you want explicit control)
            var userBadges = await _unitOfWork.UserBadge.FindAsync(ub => ub.UserId == userId);
            foreach (var userBadge in userBadges)
            {
                await _unitOfWork.UserBadge.DeleteAsync(userBadge);
            }

            var userMissions = await _unitOfWork.UserMission.FindAsync(um => um.UserId == userId);
            foreach (var userMission in userMissions)
            {
                await _unitOfWork.UserMission.DeleteAsync(userMission);
            }

            var userTreasures = await _unitOfWork.UserTreasure.FindAsync(ut => ut.UserId == userId);
            foreach (var userTreasure in userTreasures)
            {
                await _unitOfWork.UserTreasure.DeleteAsync(userTreasure);
            }

            var landmarkVisits = await _unitOfWork.LandmarkVisit.FindAsync(lv => lv.UserId == userId);
            foreach (var landmarkVisit in landmarkVisits)
            {
                await _unitOfWork.LandmarkVisit.DeleteAsync(landmarkVisit);
            }

            var stressLogs = await _unitOfWork.StressLog.FindAsync(sl => sl.UserId == userId);
            foreach (var stressLog in stressLogs)
            {
                await _unitOfWork.StressLog.DeleteAsync(stressLog);
            }

            // Finally, remove the user
            await _unitOfWork.User.DeleteAsync(user);
            await _unitOfWork.CommitAsync();

            return ServiceResult.Success("Người dùng đã được xóa thành công.");
        }

        public async Task<ServiceResult> ChangeUserPasswordAsync(Guid userId, UserChangePasswordRequestDto request)
        {
            var localAuth = await _unitOfWork.LocalAuthentication.GetFirstOrDefaultAsync(la => la.UserId == userId);
            if (localAuth == null)
            {
                return ServiceResult.Failed("HB40406", "Người dùng không có mật khẩu cục bộ để thay đổi.");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, localAuth.PasswordHash))
            {
                return ServiceResult.Failed("HB40014", "Mật khẩu hiện tại không đúng.");
            }

            localAuth.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            _unitOfWork.LocalAuthentication.Update(localAuth);
            await _unitOfWork.CommitAsync();

            return ServiceResult.Success("Mật khẩu đã được thay đổi thành công.");
        }

        public async Task<ServiceResult> AdminChangeUserPasswordAsync(Guid userId, AdminChangeUserPasswordRequestDto request)
        {
            var localAuth = await _unitOfWork.LocalAuthentication.GetFirstOrDefaultAsync(la => la.UserId == userId);
            if (localAuth == null)
            {
                // If no local password, create one
                localAuth = new LocalAuthentication
                {
                    UserId = userId,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword)
                };
                await _unitOfWork.LocalAuthentication.AddAsync(localAuth);
            }
            else
            {
                localAuth.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                _unitOfWork.LocalAuthentication.Update(localAuth);
            }
            await _unitOfWork.CommitAsync();

            return ServiceResult.Success("Mật khẩu người dùng đã được thay đổi thành công bởi quản trị viên.");
        }

        public async Task<ServiceResult> AssignRolesToUserAsync(Guid userId, List<Guid> roleIds)
        {
            var user = await _unitOfWork.User.GetFirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return ServiceResult.Failed("HB40407", "Người dùng không tồn tại.");
            }

            // Remove existing roles for the user
            var existingUserRoles = await _unitOfWork.UserRole.FindAsync(ur => ur.UserId == userId);
            foreach (var userRole in existingUserRoles)
            {
                await _unitOfWork.UserRole.DeleteAsync(userRole);
            }

            // Add new roles
            foreach (var roleId in roleIds)
            {
                var roleExists = await _unitOfWork.Role.GetFirstOrDefaultAsync(r => r.RoleId == roleId) != null;
                if (!roleExists)
                {
                    return ServiceResult.Failed("HB40015", $"Vai trò với ID {roleId} không tồn tại.");
                }
                await _unitOfWork.UserRole.AddAsync(new UserRole { UserId = userId, RoleId = roleId });
            }

            await _unitOfWork.CommitAsync();

            return ServiceResult.Success("Vai trò của người dùng đã được cập nhật thành công.");
        }

        public async Task<ServiceResult> CreateUserAndLocalAuthAsync(User newUser, LocalAuthentication newLocalAuth)
        {
            await _unitOfWork.User.AddAsync(newUser);
            await _unitOfWork.LocalAuthentication.AddAsync(newLocalAuth);
            await _unitOfWork.CommitAsync();
            return ServiceResult.Success("Người dùng và thông tin xác thực cục bộ đã được tạo thành công.");
        }

        public async Task<ServiceResult> CreateUserAndSocialAuthAsync(User newUser, SocialAuthentication newSocialAuth)
        {
            await _unitOfWork.User.AddAsync(newUser);
            await _unitOfWork.SocialAuthentication.AddAsync(newSocialAuth);
            await _unitOfWork.CommitAsync();
            return ServiceResult.Success("Người dùng và thông tin xác thực mạng xã hội đã được tạo thành công.");
        }
    }
}
