using System;
using System.Collections.Generic;
using System.Text;

namespace Rehletak.Shared.Dtos.Auth.SmsOtp
{
    public class VerifyOtpRequest
    {
        public string phoneNumper { get; set; }
        public string otp { get; set; }
    }
}
