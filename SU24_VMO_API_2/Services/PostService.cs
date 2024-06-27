﻿using BusinessObject.Models;
using Repository.Implements;
using Repository.Interfaces;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.Supporters.ExceptionSupporter;
using SU24_VMO_API.Supporters.TimeHelper;
using SU24_VMO_API_2.DTOs.Response;

namespace SU24_VMO_API.Services
{
    public class PostService
    {
        private readonly IPostRepository repository;
        private readonly IUserRepository _userRepository;
        private readonly IOrganizationManagerRepository _organizationManagerRepository;
        private readonly FirebaseService _firebaseService;

        public PostService(IPostRepository repository, FirebaseService firebaseService, IUserRepository userRepository,
            IOrganizationManagerRepository organizationManagerRepository)
        {
            this.repository = repository;
            _firebaseService = firebaseService;
            _userRepository = userRepository;
            _organizationManagerRepository = organizationManagerRepository;
        }

        public IEnumerable<Post> GetAllPosts()
        {
            return repository.GetAll();
        }

        public IEnumerable<PostResponse> GetAllPostsByOrganizationManagerId(Guid organizationManagerId)
        {
            var posts = repository.GetAllPostByOrganizationManagerId(organizationManagerId);
            var postReponses = new List<PostResponse>();

            foreach (var post in posts)
            {
                if (post.CreatePostRequest != null)
                {
                    post.CreatePostRequest = null;
                }
                var om = _organizationManagerRepository.GetById(organizationManagerId);
                postReponses.Add(new PostResponse
                {
                    PostID = post.PostID,
                    Content = post.Content,
                    Cover = post.Cover,
                    CreateAt = post.CreateAt,
                    Description = post.Description,
                    Image = post.Image,
                    IsActive = post.IsActive,
                    Title = post.Title,
                    UpdateAt = post.UpdateAt,
                    AuthorName = om!.FirstName.Trim() + " " + om!.LastName.Trim()
                });
            }
            return postReponses;
        }

        public IEnumerable<PostResponse> GetAllPostsByUserId(Guid userId)
        {
            var posts = repository.GetAllPostByUserId(userId);
            var postReponses = new List<PostResponse>();


            foreach (var post in posts)
            {
                if (post.CreatePostRequest != null)
                {
                    post.CreatePostRequest = null;
                }
                var user = _userRepository.GetById(userId);
                postReponses.Add(new PostResponse
                {
                    PostID = post.PostID,
                    Content = post.Content,
                    Cover = post.Cover,
                    CreateAt = post.CreateAt,
                    Description = post.Description,
                    Image = post.Image,
                    IsActive = post.IsActive,
                    Title = post.Title,
                    UpdateAt = post.UpdateAt,
                    AuthorName = user!.FirstName.Trim() + " " + user!.LastName.Trim()
                });
            }
            return postReponses;
        }

        public PostResponse? GetById(Guid id)
        {
            var post = repository.GetById(id);
            if (post == null) { throw new NotFoundException("Post not found!"); }
            var postResponse = new PostResponse
            {
                PostID = post.PostID,
                Content = post.Content,
                Cover = post.Cover,
                CreateAt = post.CreateAt,
                Description = post.Description,
                Image = post.Image,
                IsActive = post.IsActive,
                Title = post.Title,
                UpdateAt = post.UpdateAt
            };
            if (post.CreatePostRequest != null && post.CreatePostRequest.CreateByOM != null)
            {
                var om = _organizationManagerRepository.GetById((Guid)post.CreatePostRequest.CreateByOM);
                postResponse.AuthorName = om!.FirstName.Trim() + " " + om!.LastName.Trim();
                return postResponse;
            }
            else if (post.CreatePostRequest != null && post.CreatePostRequest.CreateByUser != null)
            {
                var user = _userRepository.GetById((Guid)post.CreatePostRequest.CreateByUser);
                postResponse.AuthorName = user!.FirstName.Trim() + " " + user!.LastName.Trim();
                return postResponse;
            }
            else throw new BadRequestException("Post existed but can not found request of this post to append name of the author!");
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
