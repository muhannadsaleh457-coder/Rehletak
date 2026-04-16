using System;
using System.Collections.Generic;
using System.Text;

namespace Rehletak.Services.Auth.Options
{
    public class TwilioOption
    {
        public string AccountSID { get; set; }
        public string AuthToken { get; set; }
        public string PhoneNumber { get; set; }
    }
}
