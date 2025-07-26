using System;
using System.Collections.Generic;

#nullable disable

namespace Repository.Models
{
    public partial class LocalAuthentication
    {
        public Guid UserId { get; set; }
        public string PasswordHash { get; set; }

        public virtual User User { get; set; }
    }
}
