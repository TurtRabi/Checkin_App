using Common;
using Dto.Landmark;
using Repository.Models;

namespace Service.LandmarkService
{
    public interface ILandmarkService
    {
        Task<ServiceResult<IEnumerable<LandmarkResponseDto>>> GetAllLandmarksAsync();
        Task<ServiceResult<LandmarkResponseDto>> GetLandmarkByIdAsync(Guid id);
        Task<ServiceResult<LandmarkResponseDto>> CreateLandmarkAsync(LandmarkCreateRequestDto request);
        Task<ServiceResult<LandmarkResponseDto>> UpdateLandmarkAsync(LandmarkUpdateRequestDto request);
        Task<ServiceResult<bool>> DeleteLandmarkAsync(Guid id);
        Task<ServiceResult<IEnumerable<LandmarkResponseDto>>> GetPendingLandmarksAsync();
        Task<ServiceResult> ApproveLandmarkAsync(Guid landmarkId);
        Task<ServiceResult> RejectLandmarkAsync(Guid landmarkId);
    }
}