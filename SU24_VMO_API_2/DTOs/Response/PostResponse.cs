namespace SU24_VMO_API_2.DTOs.Response
{
    public class PostResponse
    {
        public Guid PostID { get; set; } = default!;
        public string AuthorName { get; set; } = default!;
        public string Cover { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Content { get; set; } = default!;
        public string Image { get; set; } = default!;
        public string? Description { get; set; } = default!;
        public bool IsActive { get; set; } = default!;
        public DateTime CreateAt { get; set; } = default!;
        public DateTime? UpdateAt { get; set; } = default!;
    }
}
