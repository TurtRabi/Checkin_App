
namespace Dto.RewardCard
{
    public class UserRewardCardResponseDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid RewardCardId { get; set; }
        public DateTime CollectedAt { get; set; }
    }
}
