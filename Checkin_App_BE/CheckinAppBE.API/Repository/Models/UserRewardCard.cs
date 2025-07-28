using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repository.Models
{
    public class UserRewardCard
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid RewardCardId { get; set; }

        [Required]
        public DateTime CollectedAt { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("RewardCardId")]
        public RewardCard RewardCard { get; set; }
    }
}
