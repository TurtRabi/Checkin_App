using Common;
using Dto.LandmarkVisit;
using Repository.Models;

namespace Service.LandmarkVisitService
{
    public interface ILandmarkVisitService
    {
        Task<ServiceResult<LandmarkVisitResponseDto>> CreateLandmarkVisitAsync(Guid userId, LandmarkVisitCreateRequestDto request);
        Task<ServiceResult<IEnumerable<LandmarkVisitResponseDto>>> GetUserLandmarkVisitsAsync(Guid userId);
    }
}