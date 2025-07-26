using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    public partial class UserBadge
    {
        public Guid UserBadgeId { get; set; }
        public Guid UserId { get; set; }
        public Guid BadgeId { get; set; }
        public DateTime EarnedAt { get; set; }

        public virtual Badge Badge { get; set; }
        public virtual User User { get; set; }
    }
}
