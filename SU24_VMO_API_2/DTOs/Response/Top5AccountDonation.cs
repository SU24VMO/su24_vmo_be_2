namespace SU24_VMO_API_2.DTOs.Response
{
    public class Top5AccountDonation
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string? Avatar { get; set; } = default!;
        public string TotalDonation { get; set; } = default!;
    }
}
