using System;
using System.Collections.Generic;
using System.Text;

namespace Rehletak.Shared.Dtos.Auth.Login
{
    public class LoginRequest
    {
        public string email { get; set; }
        public string password { get; set; }
    }
}
