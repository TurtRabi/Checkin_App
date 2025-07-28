using System;
using System.Collections.Generic;

#nullable enable

namespace Repository.Models
{
    public partial class User
    {
        public User()
        {
            LandmarkVisits = new HashSet<LandmarkVisit>();
            SocialAuthentications = new HashSet<SocialAuthentication>();
            StressLogs = new HashSet<StressLog>();
            UserBadges = new HashSet<UserBadge>();
            UserMissions = new HashSet<UserMission>();
            UserRoles = new HashSet<UserRole>();
            UserTreasures = new HashSet<UserTreasure>();
            UserSessions = new HashSet<UserSession>();
            UserRewardCards = new HashSet<UserRewardCard>(); // Thêm HashSet mới
        }

        public Guid Id { get; set; }
        public required string UserName { get; set; }
        public required string DisplayName { get; set; }
        public string? Email { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int Points { get; set; } = 0;
        public int Coin { get; set; } = 0;
        public int ExperiencePoints { get; set; } = 0;
        public bool IsBanned { get; set; } = false;

        public virtual LocalAuthentication? LocalAuthentication { get; set; }
        public virtual ICollection<LandmarkVisit> LandmarkVisits { get; set; }
        public virtual ICollection<SocialAuthentication> SocialAuthentications { get; set; }
        public virtual ICollection<StressLog> StressLogs { get; set; }
        public virtual ICollection<UserBadge> UserBadges { get; set; }
        public virtual ICollection<UserMission> UserMissions { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<UserTreasure> UserTreasures { get; set; }
        public virtual ICollection<UserSession> UserSessions { get; set; }
        public virtual ICollection<UserRewardCard> UserRewardCards { get; set; } // Thêm ICollection mới
    }
}