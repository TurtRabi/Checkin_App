using Common;
using Dto.LandmarkVisit;
using Repository.Models;
using Repository.Repositories;
using Repository.UWO;
using Service.BadgeService;

namespace Service.LandmarkVisitService
{
    public class LandmarkVisitService : ILandmarkVisitService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILandmarkVisitRepository _landmarkVisitRepository;
        private readonly IBadgeService _badgeService;

        public LandmarkVisitService(IUnitOfWork unitOfWork, ILandmarkVisitRepository landmarkVisitRepository, IBadgeService badgeService)
        {
            _unitOfWork = unitOfWork;
            _landmarkVisitRepository = landmarkVisitRepository;
            _badgeService = badgeService;
        }

        public async Task<ServiceResult<LandmarkVisitResponseDto>> CreateLandmarkVisitAsync(Guid userId, LandmarkVisitCreateRequestDto request)
        {
            var landmarkVisit = new LandmarkVisit
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                LandmarkId = request.LandmarkId,
                CheckInTime = DateTime.UtcNow,
                Latitude = request.Latitude,
                Longitude = request.Longitude
            };

            await _landmarkVisitRepository.AddAsync(landmarkVisit);
            await _unitOfWork.CommitAsync();

            // Logic to award badges based on check-in count
            await AwardBadgesBasedOnCheckinCount(userId);

            var landmarkVisitDto = new LandmarkVisitResponseDto
            {
                Id = landmarkVisit.Id,
                UserId = landmarkVisit.UserId,
                LandmarkId = landmarkVisit.LandmarkId,
                CheckInTime = landmarkVisit.CheckInTime,
                Latitude = landmarkVisit.Latitude,
                Longitude = landmarkVisit.Longitude
            };
            return ServiceResult<LandmarkVisitResponseDto>.Success(landmarkVisitDto);
        }

        private async Task AwardBadgesBasedOnCheckinCount(Guid userId)
        {
            // Get total check-ins for the user
            var userCheckins = await _unitOfWork.LandmarkVisitRepository.GetByConditionAsync(lv => lv.UserId == userId);
            var totalCheckins = userCheckins.Count();

            // Get all badges that require check-ins
            var allBadges = await _unitOfWork.BadgeRepository.GetAllAsync();
            var checkinBadges = allBadges.Where(b => b.RequiredCheckins > 0);

            foreach (var badge in checkinBadges)
            {
                if (totalCheckins >= badge.RequiredCheckins)
                {
                    // Check if user already has this badge
                    var existingUserBadge = await _unitOfWork.UserBadgeRepository.GetSingleByConditionAsync(ub => ub.UserId == userId && ub.BadgeId == badge.Id);
                    if (existingUserBadge == null)
                    {
                        // Award the badge
                        var awardResult = await _badgeService.AwardUserBadgeAsync(userId, badge.Id);
                        if (awardResult.IsSuccess)
                        {
                            Console.WriteLine($"User {userId} awarded badge {badge.Name} for {totalCheckins} check-ins.");
                        }
                        else
                        {
                            Console.WriteLine($"Failed to award badge {badge.Name} to user {userId}: {awardResult.Message}");
                        }
                    }
                }
            }
        }

        public async Task<ServiceResult<IEnumerable<LandmarkVisitResponseDto>>> GetUserLandmarkVisitsAsync(Guid userId)
        {
            var landmarkVisits = await _landmarkVisitRepository.FindAsync(lv => lv.UserId == userId);
            var landmarkVisitDtos = landmarkVisits.Select(lv => new LandmarkVisitResponseDto
            {
                Id = lv.Id,
                UserId = lv.UserId,
                LandmarkId = lv.LandmarkId,
                CheckInTime = lv.CheckInTime,
                Latitude = lv.Latitude,
                Longitude = lv.Longitude
            });
            return ServiceResult<IEnumerable<LandmarkVisitResponseDto>>.Success(landmarkVisitDtos);
        }
    }
}