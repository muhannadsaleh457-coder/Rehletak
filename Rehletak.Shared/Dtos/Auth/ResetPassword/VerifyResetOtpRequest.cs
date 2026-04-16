using System;
using System.Collections.Generic;
using System.Text;

namespace Rehletak.Shared.Dtos.Auth.ResetPassword
{
    public class VerifyResetOtpRequest
    {
        public string email { get; set; }
        public string otp { get; set; }
    }
}
