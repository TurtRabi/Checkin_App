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

        public Guid TreasureId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string Rarity { get; set; }
        public Guid? LandmarkId { get; set; }

        public virtual Landmark Landmark { get; set; }
        public virtual ICollection<UserTreasure> UserTreasures { get; set; }
    }
}
