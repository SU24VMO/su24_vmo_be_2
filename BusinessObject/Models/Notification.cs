using BusinessObject.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class Notification
    {
        public Guid NotificationID { get; set; } = default!;
        public Guid AccountID { get; set; } = default!;
        public NotificationCategory NotificationCategory { get; set; } = default!;
        public string Content { get; set; } = default!;
        public DateTime CreateDate { get; set; }
        public bool IsSeen { get; set; } = default!;
        public virtual Account? Account { get; set; }
    }
}
