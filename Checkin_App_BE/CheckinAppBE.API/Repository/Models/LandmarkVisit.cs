using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    public partial class LandmarkVisit
    {
        public LandmarkVisit()
        {
            UserTreasures = new HashSet<UserTreasure>();
        }

        public Guid VisitId { get; set; }
        public Guid UserId { get; set; }
        public Guid LandmarkId { get; set; }
        public DateTime VisitTime { get; set; }
        public string Note { get; set; }
        public string ImageUrl { get; set; }

        public virtual Landmark Landmark { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<UserTreasure> UserTreasures { get; set; }
    }
}
