using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    public partial class Landmark
    {
        public Landmark()
        {
            LandmarkVisits = new HashSet<LandmarkVisit>();
            Treasures = new HashSet<Treasure>();
        }

        public Guid LandmarkId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual ICollection<LandmarkVisit> LandmarkVisits { get; set; }
        public virtual ICollection<Treasure> Treasures { get; set; }
    }
}
