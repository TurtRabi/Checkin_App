using System;
using System.ComponentModel.DataAnnotations;

namespace Dto.Treasure
{
    public class TreasureCreateRequestDto
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string Rarity { get; set; }
        public int Coin { get; set; }
        public int ExperiencePoints { get; set; }
        [Required]
        public string TreasureType { get; set; }
        public Guid? LandmarkId { get; set; }
    }
}