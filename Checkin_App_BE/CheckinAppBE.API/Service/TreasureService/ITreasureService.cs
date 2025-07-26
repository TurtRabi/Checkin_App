using Common;
using Dto.Treasure;
using Repository.Models;

namespace Service.TreasureService
{
    public interface ITreasureService
    {
        Task<ServiceResult<IEnumerable<TreasureResponseDto>>> GetAllTreasuresAsync();
        Task<ServiceResult<TreasureResponseDto>> GetTreasureByIdAsync(Guid id);
        Task<ServiceResult<TreasureResponseDto>> CreateTreasureAsync(TreasureCreateRequestDto treasureDto);
        Task<ServiceResult<TreasureResponseDto>> UpdateTreasureAsync(Guid id, TreasureUpdateRequestDto treasureDto);
        Task<ServiceResult<bool>> DeleteTreasureAsync(Guid id);
        Task<ServiceResult<OpenTreasureResponseDto>> OpenDailyTreasureAsync(Guid userId);
        Task<ServiceResult<OpenTreasureResponseDto>> OpenSpecialTreasureAsync(Guid userId, Guid visitId);
    }
}