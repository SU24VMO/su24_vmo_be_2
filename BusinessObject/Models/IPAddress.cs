using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class IPAddress
    {
        public Guid IPAddressId { get; set; } = default!;
        public DateTime? LoginTime { get; set; } = default!;
        public string? IPAddressValue { get; set; } = default!;
        public string? Longitude { get; set; } = default!;
        public string? Latitude { get; set; } = default!;
        public string? Road { get; set; } = default!;
        public string? Suburb { get; set; } = default!;
        public string? City { get; set; } = default!;
        public string? Country { get; set; } = default!;
        public string? Postcode { get; set; } = default!;
        public string? CountryCode { get; set; } = default!;
        public Guid AccountId { get; set; } = default!;
        public DateTime? CreateDate { get; set; } = default!;
        public Account? Account { get; set; }
    }
}
