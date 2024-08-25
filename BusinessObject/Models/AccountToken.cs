using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class AccountToken
    {
        public Guid AccountTokenId { get; set; }
        public Guid AccountID { get; set; }
        public string AccessToken { get; set; } = default!;
        public DateTime? ExpiredDateAccessToken { get; set; } = default!;
        public string? CodeRefreshToken { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
        public DateTime? ExpiredDateRefreshToken { get; set; } = default!;
        public DateTime CreatedDate {  get; set; } = default!;
        public virtual Account? Account { get; set; }
    }
}
