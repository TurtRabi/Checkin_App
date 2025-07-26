using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    public partial class Treasure
    {
        public Treasure()
        {
            UserTreasures = new HashSet<UserTreasure>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string Rarity { get; set; }
        public int Coin { get; set; }
        public int ExperiencePoints { get; set; }
        public string TreasureType { get; set; } // e.g., "Daily", "SpecialCheckin"
        public Guid? LandmarkId { get; set; }

        public virtual Landmark Landmark { get; set; }
        public virtual ICollection<UserTreasure> UserTreasures { get; set; }
    }
}