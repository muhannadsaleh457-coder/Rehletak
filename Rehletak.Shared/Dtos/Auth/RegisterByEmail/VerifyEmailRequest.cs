using System;
using System.Collections.Generic;
using System.Text;

namespace Rehletak.Shared.Dtos.Auth.RegisterByEmail
{
    public class VerifyEmailRequest
    {
        public string email { get; set; }
        public string otp { get; set; }
    }
}
