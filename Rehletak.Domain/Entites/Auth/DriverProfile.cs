using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Rehletak.Domain.Entites.Auth
{
    public class DriverProfile : BaseEntity<int>
    {
        public string user_id { get; set; }
        public string license_number { get; set; }
        public string car_model { get; set; }
        public string car_plate { get; set; }
        public bool is_verified { get; set; }
        public DateTime created_at { get; set; }

        public AppUser user { get; set; }
    }
}
