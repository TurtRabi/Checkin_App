using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repository.Models
{
    public class RewardCard
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [MaxLength(500)]
        public string ImageUrl { get; set; }

        [Required]
        [MaxLength(50)]
        public string Rarity { get; set; } // Common / Rare / Epic / Legendary

        [Required]
        public double DropRate { get; set; } // Xác suất rơi (ví dụ: 0.01 cho 1%)

        [MaxLength(500)]
        public string? AnimationVideoUrl { get; set; }

        [MaxLength(50)]
        public string? AnimationType { get; set; } // "video" / "gif" / "lottie"

        public virtual ICollection<UserRewardCard> UserRewardCards { get; set; } // Thêm ICollection mới
    }
}
