using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Rehletak.Shared.Dtos.Auth
{
    public class AuthResponseDto
    {
        public string email { get; set; }
        public string name { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
