using BusinessObject.Models;

namespace SU24_VMO_API_2.DTOs.Response
{
    public class AccountsWithModeratorRole
    {
        public List<Account> Accounts { get; set; }
        public int TotalItem { get; set; }
    }
}
