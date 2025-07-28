
using AutoMapper;
using Common;
using Dto.RewardCard;
using Microsoft.EntityFrameworkCore;
using Repository.Models;
using Repository.UWO;

namespace Service.RewardCardService
{
    public class RewardCardService : IRewardCardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RewardCardService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResult<RewardCardResponseDto>> CheckinRandomCard(Guid userId, Guid visitId)
        {
            var rewardCards = await _unitOfWork.RewardCardRepository.GetAllAsync();
            var random = new Random();

            foreach (var card in rewardCards)
            {
                if (random.NextDouble() < card.DropRate)
                {
                    var userRewardCard = new UserRewardCard
                    {
                        UserId = userId,
                        RewardCardId = card.Id,
                        CollectedAt = DateTime.UtcNow
                    };
                    await _unitOfWork.UserRewardCardRepository.AddAsync(userRewardCard);
                    await _unitOfWork.CommitAsync();

                    return ServiceResult<RewardCardResponseDto>.Success(_mapper.Map<RewardCardResponseDto>(card));
                }
            }

            return ServiceResult<RewardCardResponseDto>.Fail("No card dropped.", 404);
        }

        public async Task<ServiceResult<RewardCardResponseDto>> CreateRewardCard(RewardCardResponseDto rewardCardDto)
        {
            var rewardCard = _mapper.Map<RewardCard>(rewardCardDto);
            await _unitOfWork.RewardCardRepository.AddAsync(rewardCard);
            await _unitOfWork.CommitAsync();
            return ServiceResult<RewardCardResponseDto>.Success(_mapper.Map<RewardCardResponseDto>(rewardCard));
        }

        public async Task<ServiceResult> DeleteRewardCard(Guid id)
        {
            var rewardCard = await _unitOfWork.RewardCardRepository.GetByIdAsync(id);
            if (rewardCard == null)
            {
                return ServiceResult.Fail("Reward card not found.", 404);
            }

            _unitOfWork.RewardCardRepository.Delete(rewardCard);
            await _unitOfWork.CommitAsync();
            return ServiceResult.Success();
        }

        public async Task<ServiceResult<IEnumerable<RewardCardResponseDto>>> GetRewardCards()
        {
            var rewardCards = await _unitOfWork.RewardCardRepository.GetAllAsync();
            var rewardCardDtos = _mapper.Map<IEnumerable<RewardCardResponseDto>>(rewardCards);
            return ServiceResult<IEnumerable<RewardCardResponseDto>>.Success(rewardCardDtos);
        }

        public async Task<ServiceResult<IEnumerable<UserRewardCardResponseDto>>> GetUserRewardCards(Guid userId)
        {
            var userRewardCards = await _unitOfWork.UserRewardCardRepository.GetByConditionAsync(x => x.UserId == userId);
            var userRewardCardDtos = _mapper.Map<IEnumerable<UserRewardCardResponseDto>>(userRewardCards);
            return ServiceResult<IEnumerable<UserRewardCardResponseDto>>.Success(userRewardCardDtos);
        }

        public async Task<ServiceResult> UpdateRewardCard(Guid id, RewardCardResponseDto rewardCardDto)
        {
            var rewardCard = await _unitOfWork.RewardCardRepository.GetByIdAsync(id);
            if (rewardCard == null)
            {
                return ServiceResult.Fail("Reward card not found.", 404);
            }

            _mapper.Map(rewardCardDto, rewardCard);
            _unitOfWork.RewardCardRepository.Update(rewardCard);
            await _unitOfWork.CommitAsync();
            return ServiceResult.Success();
        }
    }
}
