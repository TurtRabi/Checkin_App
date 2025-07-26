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

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual ICollection<LandmarkVisit> LandmarkVisits { get; set; }
        public virtual ICollection<Treasure> Treasures { get; set; }
    }
}