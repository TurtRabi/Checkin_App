using Common;
using Dto.Treasure;
using Repository.Models;

namespace Service.UserTreasureService
{
    public interface IUserTreasureService
    {
        Task<ServiceResult<IEnumerable<UserTreasureResponseDto>>> GetUserTreasuresAsync(Guid userId);
        Task<ServiceResult<UserTreasureResponseDto>> GetUserTreasureByIdAsync(Guid id);
        Task<ServiceResult<UserTreasureResponseDto>> AddUserTreasureAsync(Guid userId, Guid treasureId, Guid? visitId = null);
    }
}