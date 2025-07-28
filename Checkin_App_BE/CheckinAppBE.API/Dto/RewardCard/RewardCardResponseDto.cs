
namespace Dto.RewardCard
{
    public class RewardCardResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string Rarity { get; set; }
        public double DropRate { get; set; }
        public string? AnimationVideoUrl { get; set; }
        public string? AnimationType { get; set; }
    }
}
