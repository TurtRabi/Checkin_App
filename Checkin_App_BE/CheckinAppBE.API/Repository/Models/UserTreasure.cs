using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    public partial class UserTreasure
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid TreasureId { get; set; }
        public DateTime CollectedAt { get; set; }
        public Guid? VisitId { get; set; }

        public virtual Treasure Treasure { get; set; }
        public virtual User User { get; set; }
        public virtual LandmarkVisit Visit { get; set; }
    }
}