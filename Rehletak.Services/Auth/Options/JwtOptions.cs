using System;
using System.Collections.Generic;
using System.Text;

namespace Rehletak.Services.Auth.Options
{
    public class JwtOptions
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public double ExpireDays { get; set; }
    }
}
