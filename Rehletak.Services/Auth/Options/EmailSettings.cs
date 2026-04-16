using System;
using System.Collections.Generic;
using System.Text;

namespace Rehletak.Services.Auth.Options
{
    public class EmailSettings
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }
}
