namespace SU24_VMO_API.DTOs.Response
{
    public class CreateTransactionResponse
    {
        public int? OrderID { get; set; } = default!;
        public string? QRCode { get; set; } = default!;
    }
}
