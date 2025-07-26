using Common;
using Dto.Badge;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.BadgeService
{
    public interface IBadgeService
    {
        Task<ServiceResult<IEnumerable<BadgeResponseDto>>> GetAllBadgesAsync();
        Task<ServiceResult<BadgeResponseDto>> GetBadgeByIdAsync(Guid id);
        Task<ServiceResult<IEnumerable<UserBadgeResponseDto>>> GetUserBadgesAsync(Guid userId);
        Task<ServiceResult<UserBadgeResponseDto>> AwardUserBadgeAsync(Guid userId, Guid badgeId);
        Task<ServiceResult<BadgeResponseDto>> CreateBadgeAsync(BadgeCreateRequestDto badgeDto);
        Task<ServiceResult<BadgeResponseDto>> UpdateBadgeAsync(BadgeUpdateRequestDto badgeDto);
        Task<ServiceResult<bool>> DeleteBadgeAsync(Guid id);
    }
}