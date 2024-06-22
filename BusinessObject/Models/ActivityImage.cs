using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class ActivityImage
    {
        public Guid ActivityImageId { get; set; } = default!;
        public Guid ActivityId { get; set; } = default!;
        public string Link { get; set; } = default!;
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsActive { get; set; } = default!;
        public Activity? Activity { get; set; }
    }
}
