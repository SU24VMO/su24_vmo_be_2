using System.ComponentModel.DataAnnotations;

namespace SU24_VMO_API.DTOs.Request
{
    public class CreateStatementFileRequest
    {
        [Required]
        public Guid StatementPhaseId { get; set; } = default!;
        [Required]
        public IFormFile StatementFile { get; set; } = default!;
    }
}
