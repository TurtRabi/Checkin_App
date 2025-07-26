using Common;
using Dto.User;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.UserService
{
    public interface IUserService
    {
        Task<ServiceResult<IEnumerable<UserResponseDto>>> GetAllUsersAsync(UserFilterRequestDto filter);
        Task<ServiceResult<UserResponseDto>> GetUserByIdAsync(Guid userId);
        
        Task<ServiceResult> UpdateUserAsync(Guid userId, UserUpdateRequestDto request);
        Task<ServiceResult> DeleteUserAsync(Guid userId);
        Task<ServiceResult> ChangeUserPasswordAsync(Guid userId, UserChangePasswordRequestDto request);
        Task<ServiceResult> AdminChangeUserPasswordAsync(Guid userId, AdminChangeUserPasswordRequestDto request);
        Task<ServiceResult> AssignRolesToUserAsync(Guid userId, List<Guid> roleIds);
        Task<ServiceResult> CreateUserAndLocalAuthAsync(User newUser, LocalAuthentication newLocalAuth);
        Task<ServiceResult> CreateUserAndSocialAuthAsync(User newUser, SocialAuthentication newSocialAuth);
        Task<ServiceResult<int>> GetUserLevelAsync(Guid userId);
    }
}