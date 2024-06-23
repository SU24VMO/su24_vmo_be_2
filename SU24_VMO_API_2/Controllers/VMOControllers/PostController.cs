using BusinessObject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SU24_VMO_API.DTOs.Response;
using SU24_VMO_API.Services;

namespace SU24_VMO_API.Controllers.VMOControllers
{
    [Route("api/post")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly PostService _postService;
        private readonly PaginationService<Post> _paginationService;


        public PostController(PostService postService, PaginationService<Post> paginationService)
        {
            _postService = postService;
            _paginationService = paginationService;
        }

        [HttpGet]
        [Route("all")]
        public IActionResult GetAllPosts(int? pageSize, int? pageNo)
        {
            try
            {
                var posts = _postService.GetAllPosts();

                var response = new ResponseMessage()
                {
                    Message = "Get successfully!",
                    Data = _paginationService.PaginateList(posts, pageSize, pageNo)
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseMessage()
                {
                    Message = $"Error: {ex.Message}"
                };
                return BadRequest(response);
            }
        }
    }
}
