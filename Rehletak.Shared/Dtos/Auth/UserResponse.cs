using System;
using System.Collections.Generic;
using System.Text;

namespace Rehletak.Shared.Dtos.Auth
{
    public class UserResponse
    {
        public string id { get; set; }
        public string userName { get; set; }
        public string? role { get; set; }
        public string? token { get; set; }
    }
}
