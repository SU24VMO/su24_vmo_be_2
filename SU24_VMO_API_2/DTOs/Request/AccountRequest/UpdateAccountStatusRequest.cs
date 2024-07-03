namespace SU24_VMO_API_2.DTOs.Request.AccountRequest
{
    public class UpdateAccountStatusRequest
    {
        public Guid AccountID { get; set; }
        public bool? IsActived { get; set; } = default!;

    }
}
