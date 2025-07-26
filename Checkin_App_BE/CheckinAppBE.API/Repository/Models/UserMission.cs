using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    public partial class UserMission
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid MissionId { get; set; }
        public string Status { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public virtual Mission Mission { get; set; }
        public virtual User User { get; set; }
    }
}