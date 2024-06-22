﻿namespace SU24_VMO_API.DTOs.Request
{
    public class UpdateProcessingPhaseRequest
    {
        public Guid ProcessingPhaseId { get; set; } = default!;
        public string? Name { get; set; } = default!;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsProcessing { get; set; } = default!;
        public bool? IsEnd { get; set; } = default!;
    }
}