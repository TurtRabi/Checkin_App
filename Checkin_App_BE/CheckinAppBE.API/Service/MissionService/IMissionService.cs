using Common;
using Dto.Mission;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.MissionService
{
    public interface IMissionService
    {
        Task<ServiceResult<IEnumerable<MissionResponseDto>>> GetAllMissionsAsync();
        Task<ServiceResult<MissionResponseDto>> GetMissionByIdAsync(Guid id);
        Task<ServiceResult<IEnumerable<UserMissionResponseDto>>> GetUserMissionsAsync(Guid userId);
        Task<ServiceResult<UserMissionResponseDto>> AssignUserMissionAsync(Guid userId, Guid missionId);
        Task<ServiceResult<UserMissionResponseDto>> CompleteUserMissionAsync(Guid userMissionId);
        Task<ServiceResult<MissionResponseDto>> CreateMissionAsync(MissionCreateRequestDto missionDto);
        Task<ServiceResult<MissionResponseDto>> UpdateMissionAsync(MissionUpdateRequestDto missionDto);
        Task<ServiceResult<bool>> DeleteMissionAsync(Guid id);
        Task<ServiceResult<bool>> AssignDailyMissionsAsync(Guid userId);
    }
}