using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class Post
    {
        public Guid PostID { get; set; } = default!;
        public string Cover { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Content { get; set; } = default!;
        public string Image { get; set; } = default!;
        public string? Description { get; set; } = default!;
        public bool IsActive { get; set; } = default!;
        public DateTime CreateAt { get; set; } = default!;
        public DateTime? UpdateAt { get; set; } = default!;
        public CreatePostRequest? CreatePostRequest { get; set; }
    }
}
