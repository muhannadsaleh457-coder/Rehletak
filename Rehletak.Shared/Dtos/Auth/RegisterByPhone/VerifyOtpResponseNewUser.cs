using System;
using System.Collections.Generic;
using System.Text;

namespace Rehletak.Shared.Dtos.Auth.SmsOtp
{
    public class VerifyOtpResponseNewUser : VerifyOtpResponse
    {
        public string tempToken { get; set; }
    }
}
