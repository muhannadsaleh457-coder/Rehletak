using System;
using System.Collections.Generic;
using System.Text;

namespace Rehletak.Shared.Dtos.Auth.Login
{
    public class LoginResponse
    {
        public string accessToken { get; set; }
        public string refreshToken { get; set; }
    }
}
