using System.ComponentModel.DataAnnotations;

namespace SU24_VMO_API.DTOs.Request
{
    public class CheckTransactionRequest
    {
        [Required]
        public int OrderID { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        [Required]
        public string Email { get; set; } = default!;
    }
}
