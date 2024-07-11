using System.ComponentModel.DataAnnotations;
using BusinessObject.Models;

namespace SU24_VMO_API.DTOs.Request
{
    public class CreateStatementFileRequest
    {
        [Required]
        public Guid StatementPhaseId { get; set; } = default!;
        [Required]
        public IFormFile[] StatementFile { get; set; } = default!;
        [Required]
        public Guid AccountId { get; set; } = default!;
    }
}
