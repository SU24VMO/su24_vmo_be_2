using BusinessObject.Models;

namespace SU24_VMO_API_2.DTOs.Response
{
    public class CreatePostRequestByPostTitle
    {
        public List<CreatePostRequest>? CreatePostRequests { get; set; }
        public int TotalItem { get; set; }
    }
}
