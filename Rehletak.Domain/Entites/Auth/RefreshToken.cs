using System;
using System.Collections.Generic;
using System.Text;

namespace Rehletak.Domain.Entites.Auth
{
    public class RefreshToken : BaseEntity<int>
    {

        public string token { get; set; }
        public DateTime expires_at { get; set; }
        public DateTime? revoked_at { get; set; }
        public DateTime created_at { get; set; } = DateTime.UtcNow;
        public string userId { get; set; }
        public bool isExpires => DateTime.Now >= expires_at;
        public bool isActive => revoked_at == null && !isExpires;

        public AppUser user { get; set; }
    }
}
