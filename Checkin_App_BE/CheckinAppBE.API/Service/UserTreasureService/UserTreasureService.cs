using AutoMapper;
using Common;
using Dto.Treasure;
using Repository.Models;
using Repository.UWO;

namespace Service.UserTreasureService
{
    public class UserTreasureService : IUserTreasureService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserTreasureService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResult<IEnumerable<UserTreasureResponseDto>>> GetUserTreasuresAsync(Guid userId)
        {
            var userTreasures = await _unitOfWork.UserTreasureRepository.FindAsync(ut => ut.UserId == userId, includeProperties: "Treasure,User");
            var userTreasureDtos = _mapper.Map<IEnumerable<UserTreasureResponseDto>>(userTreasures);
            return ServiceResult<IEnumerable<UserTreasureResponseDto>>.Success(userTreasureDtos);
        }

        public async Task<ServiceResult<UserTreasureResponseDto>> GetUserTreasureByIdAsync(Guid id)
        {
            var userTreasure = await _unitOfWork.UserTreasureRepository.GetByIdAsync(id, includeProperties: "Treasure,User");
            if (userTreasure == null)
            {
                return ServiceResult<UserTreasureResponseDto>.Fail("User treasure not found.");
            }
            var userTreasureDto = _mapper.Map<UserTreasureResponseDto>(userTreasure);
            return ServiceResult<UserTreasureResponseDto>.Success(userTreasureDto);
        }

        public async Task<ServiceResult<UserTreasureResponseDto>> AddUserTreasureAsync(Guid userId, Guid treasureId, Guid? visitId = null)
        {
            var userTreasure = new UserTreasure
            {
                UserId = userId,
                TreasureId = treasureId,
                CollectedAt = DateTime.UtcNow,
                VisitId = visitId
            };
            await _unitOfWork.UserTreasureRepository.AddAsync(userTreasure);
            await _unitOfWork.CommitAsync();

            // Reload the userTreasure with navigation properties for the DTO
            var createdUserTreasure = await _unitOfWork.UserTreasureRepository.GetByIdAsync(userTreasure.Id, includeProperties: "Treasure,User");

            return ServiceResult<UserTreasureResponseDto>.Success(_mapper.Map<UserTreasureResponseDto>(createdUserTreasure));
        }
    }
}
