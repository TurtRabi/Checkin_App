using Common;
using Dto.Mission;
using Repository.Models;
using Repository.UWO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.MissionService
{
    public class MissionService : IMissionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MissionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<IEnumerable<MissionResponseDto>>> GetAllMissionsAsync()
        {
            var missions = await _unitOfWork.MissionRepository.GetAllAsync();
            var missionDtos = missions.Select(m => new MissionResponseDto
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                CompletionCriteria = m.CompletionCriteria,
                IsDailyMission = m.IsDailyMission,
                TargetValue = m.TargetValue,
                PointsAwarded = m.PointsAwarded
            });
            return ServiceResult<IEnumerable<MissionResponseDto>>.Success(missionDtos);
        }

        public async Task<ServiceResult<MissionResponseDto>> GetMissionByIdAsync(Guid id)
        {
            var mission = await _unitOfWork.MissionRepository.GetByIdAsync(id);
            if (mission == null)
            {
                return ServiceResult<MissionResponseDto>.Fail("Mission not found.");
            }
            var missionDto = new MissionResponseDto
            {
                Id = mission.Id,
                Title = mission.Title,
                Description = mission.Description,
                CompletionCriteria = mission.CompletionCriteria,
                IsDailyMission = mission.IsDailyMission,
                TargetValue = mission.TargetValue,
                PointsAwarded = mission.PointsAwarded
            };
            return ServiceResult<MissionResponseDto>.Success(missionDto);
        }

        public async Task<ServiceResult<IEnumerable<UserMissionResponseDto>>> GetUserMissionsAsync(Guid userId)
        {
            var userMissions = await _unitOfWork.UserMissionRepository.GetByConditionAsync(um => um.UserId == userId, includeProperties: "Mission");
            var userMissionDtos = userMissions.Select(um => new UserMissionResponseDto
            {
                Id = um.Id,
                UserId = um.UserId,
                MissionId = um.MissionId,
                Status = um.Status,
                StartedAt = um.StartedAt,
                CompletedAt = um.CompletedAt,
                Mission = new MissionResponseDto
                {
                    Id = um.Mission.Id,
                    Title = um.Mission.Title,
                    Description = um.Mission.Description,
                    CompletionCriteria = um.Mission.CompletionCriteria,
                    IsDailyMission = um.Mission.IsDailyMission,
                    TargetValue = um.Mission.TargetValue,
                    PointsAwarded = um.Mission.PointsAwarded
                }
            });
            return ServiceResult<IEnumerable<UserMissionResponseDto>>.Success(userMissionDtos);
        }

        public async Task<ServiceResult<UserMissionResponseDto>> AssignUserMissionAsync(Guid userId, Guid missionId)
        {
            var existingUserMission = await _unitOfWork.UserMissionRepository.GetSingleByConditionAsync(um => um.UserId == userId && um.MissionId == missionId && um.Status != "Completed");
            if (existingUserMission != null)
            {
                return ServiceResult<UserMissionResponseDto>.Fail("User already has this mission assigned or in progress.");
            }

            var mission = await _unitOfWork.MissionRepository.GetByIdAsync(missionId);
            if (mission == null)
            {
                return ServiceResult<UserMissionResponseDto>.Fail("Mission not found.");
            }

            var userMission = new UserMission
            {
                UserId = userId,
                MissionId = missionId,
                Status = "Assigned",
                StartedAt = DateTime.UtcNow
            };

            await _unitOfWork.UserMissionRepository.AddAsync(userMission);
            await _unitOfWork.CommitAsync();

            var userMissionDto = new UserMissionResponseDto
            {
                Id = userMission.Id,
                UserId = userMission.UserId,
                MissionId = userMission.MissionId,
                Status = userMission.Status,
                StartedAt = userMission.StartedAt,
                CompletedAt = userMission.CompletedAt,
                Mission = new MissionResponseDto
                {
                    Id = mission.Id,
                    Title = mission.Title,
                    Description = mission.Description,
                    CompletionCriteria = mission.CompletionCriteria,
                    IsDailyMission = mission.IsDailyMission,
                    TargetValue = mission.TargetValue,
                    PointsAwarded = mission.PointsAwarded
                }
            };

            return ServiceResult<UserMissionResponseDto>.Success(userMissionDto);
        }

        public async Task<ServiceResult<UserMissionResponseDto>> CompleteUserMissionAsync(Guid userMissionId)
        {
            var userMission = await _unitOfWork.UserMissionRepository.GetByIdAsync(userMissionId);
            if (userMission == null)
            {
                return ServiceResult<UserMissionResponseDto>.Fail("User mission not found.");
            }

            if (userMission.Status == "Completed")
            {
                return ServiceResult<UserMissionResponseDto>.Fail("Mission already completed.");
            }

            userMission.Status = "Completed";
            userMission.CompletedAt = DateTime.UtcNow;

            _unitOfWork.UserMissionRepository.Update(userMission);
            await _unitOfWork.CommitAsync();

            var mission = await _unitOfWork.MissionRepository.GetByIdAsync(userMission.MissionId);
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userMission.UserId);

            if (user != null && mission != null)
            {
                user.Points += mission.PointsAwarded;
                _unitOfWork.UserRepository.Update(user);
                await _unitOfWork.CommitAsync();
            }

            var userMissionDto = new UserMissionResponseDto
            {
                Id = userMission.Id,
                UserId = userMission.UserId,
                MissionId = userMission.MissionId,
                Status = userMission.Status,
                StartedAt = userMission.StartedAt,
                CompletedAt = userMission.CompletedAt,
                Mission = new MissionResponseDto
                {
                    Id = mission?.Id ?? Guid.Empty,
                    Title = mission?.Title,
                    Description = mission?.Description,
                    CompletionCriteria = mission?.CompletionCriteria,
                    IsDailyMission = mission?.IsDailyMission ?? false,
                    TargetValue = mission?.TargetValue ?? 0,
                    PointsAwarded = mission?.PointsAwarded ?? 0
                }
            };

            return ServiceResult<UserMissionResponseDto>.Success(userMissionDto);
        }

        public async Task<ServiceResult<MissionResponseDto>> CreateMissionAsync(MissionCreateRequestDto missionDto)
        {
            var mission = new Mission
            {
                Id = Guid.NewGuid(),
                Title = missionDto.Title,
                Description = missionDto.Description,
                CompletionCriteria = missionDto.CompletionCriteria,
                IsDailyMission = missionDto.IsDailyMission,
                TargetValue = missionDto.TargetValue,
                PointsAwarded = missionDto.PointsAwarded
            };

            await _unitOfWork.MissionRepository.AddAsync(mission);
            await _unitOfWork.CommitAsync();

            var createdMissionDto = new MissionResponseDto
            {
                Id = mission.Id,
                Title = mission.Title,
                Description = mission.Description,
                CompletionCriteria = mission.CompletionCriteria,
                IsDailyMission = mission.IsDailyMission,
                TargetValue = mission.TargetValue,
                PointsAwarded = mission.PointsAwarded
            };
            return ServiceResult<MissionResponseDto>.Success(createdMissionDto);
        }

        public async Task<ServiceResult<MissionResponseDto>> UpdateMissionAsync(MissionUpdateRequestDto missionDto)
        {
            var mission = await _unitOfWork.MissionRepository.GetByIdAsync(missionDto.Id);
            if (mission == null)
            {
                return ServiceResult<MissionResponseDto>.Fail("Mission not found.");
            }

            mission.Title = missionDto.Title;
            mission.Description = missionDto.Description;
            mission.CompletionCriteria = missionDto.CompletionCriteria;
            mission.IsDailyMission = missionDto.IsDailyMission;
            mission.TargetValue = missionDto.TargetValue;
            mission.PointsAwarded = missionDto.PointsAwarded;

            _unitOfWork.MissionRepository.Update(mission);
            await _unitOfWork.CommitAsync();

            var updatedMissionDto = new MissionResponseDto
            {
                Id = mission.Id,
                Title = mission.Title,
                Description = mission.Description,
                CompletionCriteria = mission.CompletionCriteria,
                IsDailyMission = mission.IsDailyMission,
                TargetValue = mission.TargetValue,
                PointsAwarded = mission.PointsAwarded
            };
            return ServiceResult<MissionResponseDto>.Success(updatedMissionDto);
        }

        public async Task<ServiceResult<bool>> DeleteMissionAsync(Guid id)
        {
            var mission = await _unitOfWork.MissionRepository.GetByIdAsync(id);
            if (mission == null)
            {
                return ServiceResult<bool>.Fail("Mission not found.");
            }

            _unitOfWork.MissionRepository.Delete(mission);
            await _unitOfWork.CommitAsync();
            return ServiceResult<bool>.Success(true);
        }

        public async Task<ServiceResult<bool>> AssignDailyMissionsAsync(Guid userId)
        {
            var dailyMissions = await _unitOfWork.MissionRepository.GetByConditionAsync(m => m.IsDailyMission);
            var today = DateTime.UtcNow.Date;

            foreach (var mission in dailyMissions)
            {
                // Check if the user already has this daily mission assigned for today
                var existingUserMission = await _unitOfWork.UserMissionRepository.GetSingleByConditionAsync(
                    um => um.UserId == userId && um.MissionId == mission.Id && um.StartedAt.Date == today
                );

                if (existingUserMission == null)
                {
                    // Assign the daily mission to the user
                    var userMission = new UserMission
                    {
                        UserId = userId,
                        MissionId = mission.Id,
                        Status = "Assigned",
                        StartedAt = DateTime.UtcNow
                    };
                    await _unitOfWork.UserMissionRepository.AddAsync(userMission);
                }
            }
            await _unitOfWork.CommitAsync();
            return ServiceResult<bool>.Success(true);
        }
    }
}