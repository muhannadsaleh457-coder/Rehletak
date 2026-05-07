using System;
using System.Collections.Generic;
using System.Text;

namespace Rehletak.Shared.Dtos.Auth
{
    public class UpdateUserRequest
    {
        public string newFullName { get; set; }
        public string newEmail { get; set; }
        public string newPhoneNumber { get; set; }
    }
}
