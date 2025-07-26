using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    public partial class StressLog
    {
        public Guid LogId { get; set; }
        public Guid UserId { get; set; }
        public int StressLevel { get; set; }
        public DateTime LogTime { get; set; }
        public string Notes { get; set; }

        public virtual User User { get; set; }
    }
}
