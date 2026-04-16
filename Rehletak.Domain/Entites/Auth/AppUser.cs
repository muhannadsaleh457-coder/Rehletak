using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Rehletak.Domain.Entites.Auth
{
    public class AppUser : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string full_name { get; set; }
        public string? avatar_url { get; set; }
        public UserRole role { get; set; }
        public UserProvider auth_provider { get; set; }
        public string? google_id { get; set; }
        public string? apple_id { get; set; }
        public bool is_phone_verified { get; set; } = false;
        public bool is_email_verified { get; set; } = false;
        public bool is_onboarding_complete { get; set; } = false;
        [Column(TypeName = "decimal(3,2)")]
        public decimal rating { get; set; } 
        public DateTime created_at { get; set; }

        public List<RefreshToken> refreshTokens { get; set; }
    }
}
