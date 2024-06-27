using BusinessObject.Models;
using Repository.Implements;
using Repository.Interfaces;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.Supporters.TimeHelper;

namespace SU24_VMO_API.Services
{
    public class PostService
    {
        private readonly IPostRepository repository;
        private readonly FirebaseService _firebaseService;

        public PostService(IPostRepository repository, FirebaseService firebaseService)
        {
            this.repository = repository;
            _firebaseService = firebaseService;
        }

        public IEnumerable<Post> GetAllPosts()
        {
            return repository.GetAll();
        }

        public IEnumerable<Post> GetAllPostsByOrganizationManagerId(Guid organizationManagerId)
        {
            var posts = repository.GetAllPostByOrganizationManagerId(organizationManagerId);
            foreach (var post in posts)
            {
                if (post.CreatePostRequest != null)
                {
                    post.CreatePostRequest = null;
                }
            }
            return posts;
        }

        public Post? GetById(Guid id)
        {
            return repository.GetById(id);
        }

        public async Task<Post?> CreateNewPost(CreateNewPost request)
        {
            var post = new Post
            {
                PostID = Guid.NewGuid(),
                Cover = await _firebaseService.UploadImage(request.Cover),
                Title = request.Title,
                Content = request.Content,
                Image = await _firebaseService.UploadImage(request.Image),
                CreateAt = TimeHelper.GetTime(DateTime.UtcNow),
            };
            return repository.Save(post);
        }

        public async void UpdateUpdatePostRequest(UpdatePostRequest request)
        {
            var post = repository.GetById(request.PostId);
            if (post != null)
            {
                if (request.Cover != null)
                {
                    post.Cover = await _firebaseService.UploadImage(request.Cover);
                }
                if (!String.IsNullOrEmpty(request.Title))
                {
                    post.Title = request.Title;
                }
                if (!String.IsNullOrEmpty(request.Content))
                {
                    post.Content = request.Content;
                }
                if (request.Image != null)
                {
                    post.Image = await _firebaseService.UploadImage(request.Image);
                }
                repository.Update(post);
            }
        }
    }
}
