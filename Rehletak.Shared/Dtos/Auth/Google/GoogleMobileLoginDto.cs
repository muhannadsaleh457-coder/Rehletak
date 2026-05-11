using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Rehletak.Shared.Dtos.Auth.Google
{
    public class GoogleMobileLoginDto
    {
        [Required]
        public string IdToken { get; set; } = string.Empty;
    }
}
