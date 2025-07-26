using AutoMapper;
using Common;
using Dto.Treasure;
using Repository.Models;
using Repository.UWO;
using Service.UserService;

namespace Service.TreasureService
{
    public class TreasureService : ITreasureService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public TreasureService(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<ServiceResult<IEnumerable<TreasureResponseDto>>> GetAllTreasuresAsync()
        {
            var treasures = await _unitOfWork.TreasureRepository.GetAllAsync();
            var treasureDtos = _mapper.Map<IEnumerable<TreasureResponseDto>>(treasures);
            return ServiceResult<IEnumerable<TreasureResponseDto>>.Success(treasureDtos);
        }

        public async Task<ServiceResult<TreasureResponseDto>> GetTreasureByIdAsync(Guid id)
        {
            var treasure = await _unitOfWork.TreasureRepository.GetByIdAsync(id);
            if (treasure == null)
            {
                return ServiceResult<TreasureResponseDto>.Fail("Treasure not found.");
            }
            var treasureDto = _mapper.Map<TreasureResponseDto>(treasure);
            return ServiceResult<TreasureResponseDto>.Success(treasureDto);
        }

        public async Task<ServiceResult<TreasureResponseDto>> CreateTreasureAsync(TreasureCreateRequestDto treasureDto)
        {
            var treasure = _mapper.Map<Treasure>(treasureDto);
            await _unitOfWork.TreasureRepository.AddAsync(treasure);
            await _unitOfWork.CommitAsync();
            return ServiceResult<TreasureResponseDto>.Success(_mapper.Map<TreasureResponseDto>(treasure));
        }

        public async Task<ServiceResult<TreasureResponseDto>> UpdateTreasureAsync(Guid id, TreasureUpdateRequestDto treasureDto)
        {
            var treasure = await _unitOfWork.TreasureRepository.GetByIdAsync(id);
            if (treasure == null)
            {
                return ServiceResult<TreasureResponseDto>.Fail("Treasure not found.");
            }

            _mapper.Map(treasureDto, treasure);
            _unitOfWork.TreasureRepository.Update(treasure);
            await _unitOfWork.CommitAsync();
            return ServiceResult<TreasureResponseDto>.Success(_mapper.Map<TreasureResponseDto>(treasure));
        }

        public async Task<ServiceResult<bool>> DeleteTreasureAsync(Guid id)
        {
            var treasure = await _unitOfWork.TreasureRepository.GetByIdAsync(id);
            if (treasure == null)
            {
                return ServiceResult<bool>.Fail("Treasure not found.");
            }

            _unitOfWork.TreasureRepository.Delete(treasure);
            await _unitOfWork.CommitAsync();
            return ServiceResult<bool>.Success(true);
        }

        public async Task<ServiceResult<OpenTreasureResponseDto>> OpenDailyTreasureAsync(Guid userId)
        {
            var userResult = await _userService.GetUserByIdAsync(userId);
            if (!userResult.IsSuccess)
            {
                return ServiceResult<OpenTreasureResponseDto>.Fail(userResult.Message);
            }

            var user = userResult.Data;

            // Check if user has already opened a daily treasure today
            var lastDailyTreasure = (await _unitOfWork.UserTreasureRepository
                .FindAsync(ut => ut.UserId == userId && ut.Treasure.TreasureType == "Daily"))
                .OrderByDescending(ut => ut.CollectedAt)
                .FirstOrDefault();

            if (lastDailyTreasure != null && lastDailyTreasure.CollectedAt.Date == DateTime.UtcNow.Date)
            {
                return ServiceResult<OpenTreasureResponseDto>.Fail("You have already opened your daily treasure today.");
            }

            // Get all daily treasures
            var dailyTreasures = (await _unitOfWork.TreasureRepository
                .FindAsync(t => t.TreasureType == "Daily")).ToList();

            if (!dailyTreasures.Any())
            {
                return ServiceResult<OpenTreasureResponseDto>.Fail("No daily treasures available.");
            }

            // Randomly select a daily treasure
            var random = new Random();
            var selectedTreasure = dailyTreasures[random.Next(dailyTreasures.Count)];

            // Add user treasure entry
            var userTreasure = new UserTreasure
            {
                UserId = userId,
                TreasureId = selectedTreasure.Id,
                CollectedAt = DateTime.UtcNow
            };
            await _unitOfWork.UserTreasureRepository.AddAsync(userTreasure);

            // Update user's coin and experience points
            user.Coin += selectedTreasure.Coin;
            user.ExperiencePoints += selectedTreasure.ExperiencePoints;
            _unitOfWork.UserRepository.Update(_mapper.Map<User>(user)); // Assuming UserRepository is available and UserDto can be mapped back to User model

            await _unitOfWork.CommitAsync();

            return ServiceResult<OpenTreasureResponseDto>.Success(new OpenTreasureResponseDto
            {
                UserTreasureId = userTreasure.Id,
                TreasureName = selectedTreasure.Name,
                AwardedCoin = selectedTreasure.Coin,
                AwardedExperiencePoints = selectedTreasure.ExperiencePoints,
                Message = $"Congratulations! You opened a daily treasure and received {selectedTreasure.Coin} coins and {selectedTreasure.ExperiencePoints} experience points."
            });
        }

        public async Task<ServiceResult<OpenTreasureResponseDto>> OpenSpecialTreasureAsync(Guid userId, Guid visitId)
        {
            var userResult = await _userService.GetUserByIdAsync(userId);
            if (!userResult.IsSuccess)
            {
                return ServiceResult<OpenTreasureResponseDto>.Fail(userResult.Message);
            }

            var user = userResult.Data;

            // Check if the visit is valid and associated with a special treasure
            var landmarkVisit = await _unitOfWork.LandmarkVisitRepository.GetByIdAsync(visitId);
            if (landmarkVisit == null || landmarkVisit.UserId != userId)
            {
                return ServiceResult<OpenTreasureResponseDto>.Fail("Invalid visit ID or visit not associated with this user.");
            }

            // Check if a special treasure is linked to this landmark or visit
            var specialTreasure = await _unitOfWork.TreasureRepository
                .GetFirstOrDefaultAsync(t => t.TreasureType == "SpecialCheckin" && t.LandmarkId == landmarkVisit.LandmarkId, "");

            if (specialTreasure == null)
            {
                return ServiceResult<OpenTreasureResponseDto>.Fail("No special treasure available for this check-in.");
            }

            // Check if user has already collected this special treasure for this visit
            var existingUserTreasure = await _unitOfWork.UserTreasureRepository
                .GetFirstOrDefaultAsync(ut => ut.UserId == userId && ut.TreasureId == specialTreasure.Id && ut.VisitId == visitId, "");

            if (existingUserTreasure != null)
            {
                return ServiceResult<OpenTreasureResponseDto>.Fail("You have already collected this special treasure for this visit.");
            }

            // Add user treasure entry
            var userTreasure = new UserTreasure
            {
                UserId = userId,
                TreasureId = specialTreasure.Id,
                CollectedAt = DateTime.UtcNow,
                VisitId = visitId
            };
            await _unitOfWork.UserTreasureRepository.AddAsync(userTreasure);

            // Update user's coin and experience points
            user.Coin += specialTreasure.Coin;
            user.ExperiencePoints += specialTreasure.ExperiencePoints;
            _unitOfWork.UserRepository.Update(_mapper.Map<User>(user)); // Assuming UserRepository is available and UserDto can be mapped back to User model

            await _unitOfWork.CommitAsync();

            return ServiceResult<OpenTreasureResponseDto>.Success(new OpenTreasureResponseDto
            {
                UserTreasureId = userTreasure.Id,
                TreasureName = specialTreasure.Name,
                AwardedCoin = specialTreasure.Coin,
                AwardedExperiencePoints = specialTreasure.ExperiencePoints,
                Message = $"Congratulations! You unlocked a special treasure and received {specialTreasure.Coin} coins and {specialTreasure.ExperiencePoints} experience points."
            });
        }
    }
}
