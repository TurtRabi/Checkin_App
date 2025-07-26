using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repository.Models
{
    public class SocialAuthentication
    {
        [Key]
        public Guid SocialAuthId { get; set; }

        public Guid UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Provider { get; set; } // e.g., Google, Facebook

        [Required]
        [MaxLength(255)]
        public required string ProviderKey { get; set; } // Unique ID from the social provider

        public DateTime CreatedAt { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}