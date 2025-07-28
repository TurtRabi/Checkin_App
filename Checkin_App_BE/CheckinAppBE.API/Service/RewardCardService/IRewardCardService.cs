
using Common;
using Dto.RewardCard;

namespace Service.RewardCardService
{
    public interface IRewardCardService
    {
        Task<ServiceResult<IEnumerable<RewardCardResponseDto>>> GetRewardCards();
        Task<ServiceResult<RewardCardResponseDto>> CreateRewardCard(RewardCardResponseDto rewardCard);
        Task<ServiceResult> UpdateRewardCard(Guid id, RewardCardResponseDto rewardCard);
        Task<ServiceResult> DeleteRewardCard(Guid id);
        Task<ServiceResult<IEnumerable<UserRewardCardResponseDto>>> GetUserRewardCards(Guid userId);
        Task<ServiceResult<RewardCardResponseDto>> CheckinRandomCard(Guid userId, Guid visitId);
    }
}
