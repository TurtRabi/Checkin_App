using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    public partial class Badge
    {
        public Badge()
        {
            UserBadges = new HashSet<UserBadge>();
        }

        public Guid BadgeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }

        public virtual ICollection<UserBadge> UserBadges { get; set; }
    }
}
