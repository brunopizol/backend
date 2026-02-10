using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backend.Domain.Entities
{
    public class UserSecurity
    {
        public Guid UserId { get; set; }
        public int FailedLoginCount { get; set; }
        public DateTime? LockoutEnd { get; set; }
        public bool MfaEnabled { get; set; }
        public string? TOTPSecret { get; set; } // base32 (criptografado em repouso)
    }
}
