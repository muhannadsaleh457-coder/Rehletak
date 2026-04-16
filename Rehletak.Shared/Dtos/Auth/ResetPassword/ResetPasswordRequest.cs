using System;
using System.Collections.Generic;
using System.Text;

namespace Rehletak.Shared.Dtos.Auth.ResetPassword
{
    public class ResetPasswordRequest
    {
        public string email { get; set; }
        public string newPassword { get; set; }
    }
}
