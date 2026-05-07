using System;
using System.Collections.Generic;
using System.Text;

namespace Rehletak.Shared.Dtos.Auth
{
    public class UserResponse
    {
        public string id { get; set; }
        public string fullName { get; set; }
        public string phoneNumber { get; set; }
        public string email { get; set; }
        public string? role { get; set; }
        public string? token { get; set; }
    }
}
