using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class IPAddress
    {
        public Guid IPAddressId { get; set; }
        public DateTime LoginTime { get; set; }
        public string IPAddressValue { get; set; }
        public Guid AccountId { get; set; }
        public DateTime CreateDate { get; set; }
        public Account? Account { get; set; }
    }
}
