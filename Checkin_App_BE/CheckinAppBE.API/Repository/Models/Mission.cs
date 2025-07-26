using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    public partial class Mission
    {
        public Mission()
        {
            UserMissions = new HashSet<UserMission>();
        }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CompletionCriteria { get; set; }
        public bool IsDailyMission { get; set; }
        public int TargetValue { get; set; }
        public int PointsAwarded { get; set; } = 0;

        public virtual ICollection<UserMission> UserMissions { get; set; }
    }
}