using System;
using System.Collections.Generic;
using System.Text;

namespace Rehletak.Shared.Dtos.Auth.SmsOtp
{
    public class VerifyOtpResponseOldUser : VerifyOtpResponse
    {
        public string accessToken { get; set; }
        public string refreshToken { get; set; }
        public UserResponse user { get; set; }
    }
}
