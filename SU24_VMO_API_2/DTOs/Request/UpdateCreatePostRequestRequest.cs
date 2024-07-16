﻿using System.ComponentModel.DataAnnotations;

namespace SU24_VMO_API_2.DTOs.Request
{
    public class UpdateCreatePostRequestRequest
    {
        public IFormFile? Cover { get; set; } = default!;
        public string? Title { get; set; } = default!;
        public string? Content { get; set; } = default!;
        public IFormFile? Image { get; set; } = default!;
        public string? Description { get; set; } = default!;
    }
}