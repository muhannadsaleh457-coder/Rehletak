using System;
using System.Collections.Generic;
using System.Text;

namespace Rehletak.Shared.Dtos.Auth.RegisterByEmail
{
    public class RegisterByEmailRequest
    {
        public string fullName { get; set; }
        public string phoneNumber { get; set; }
        public string email { get; set; }
        public string password { get; set; }
    }
}
