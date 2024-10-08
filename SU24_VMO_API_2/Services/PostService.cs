﻿using BusinessObject.Enums;
using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Repository.Implements;
using Repository.Interfaces;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.Supporters.ExceptionSupporter;
using SU24_VMO_API.Supporters.TimeHelper;
using SU24_VMO_API_2.DTOs.Request;
using SU24_VMO_API_2.DTOs.Response;
using SU24_VMO_API_2.Supporters.ExceptionSupporter;

namespace SU24_VMO_API.Services
{
    public class PostService
    {
        private readonly IPostRepository repository;
        private readonly IMemberRepository _memberRepository;
        private readonly IOrganizationManagerRepository _organizationManagerRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICreatePostRequestRepository _createPostRequestRepository;
        private readonly FirebaseService _firebaseService;

        public PostService(IPostRepository repository, FirebaseService firebaseService, IMemberRepository memberRepository,
            IOrganizationManagerRepository organizationManagerRepository, ICreatePostRequestRepository createPostRequestRepository,
            IAccountRepository accountRepository)
        {
            this.repository = repository;
            _firebaseService = firebaseService;
            _memberRepository = memberRepository;
            _organizationManagerRepository = organizationManagerRepository;
            _createPostRequestRepository = createPostRequestRepository;
            _accountRepository = accountRepository;
        }

        public IEnumerable<Post> GetAllPosts()
        {
            return repository.GetAll();
        }

        public IEnumerable<Post> GetAllPostsIsActive()
        {
            return repository.GetAll().Where(p => p.IsActive);
        }

        public IEnumerable<Post> GetAllPostsWithPostTitle(string? title)
        {
            if (!String.IsNullOrEmpty(title))
                return repository.GetAll().Where(p => p.Title.ToLower().Contains(title.Trim().ToLower()));
            else return repository.GetAll();
        }

        public PostCreateByOM? GetAllPostsByOrganizationManagerId(Guid organizationManagerId, string? title, int? pageSize, int? pageNo)
        {
            if (!String.IsNullOrEmpty(title))
            {
                var posts = repository.GetAllPostByOrganizationManagerId(organizationManagerId, pageSize, pageNo);
                var postReponses = new List<PostResponse>();

                foreach (var post in posts)
                {
                    var request = _createPostRequestRepository.GetCreatePostRequestByPostId(post.PostID);
                    if (request != null)
                    {
                        request.Post = null;
                        request.OrganizationManager = null;
                        request.Moderator = null;
                        request.Member = null;
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
                        IsDisable = post.IsDisable,
                        Title = post.Title,
                        UpdateAt = post.UpdateAt,
                        AuthorName = om!.FirstName.Trim() + " " + om!.LastName.Trim(),
                        CreatePostRequest = request

                    });
                }

                return new PostCreateByOM
                {
                    Posts = postReponses.Where(p => p.Title.ToLower().Contains(title.ToLower().Trim()) && p.IsDisable == false).ToList(),
                    TotalItem = repository.GetAllPostByOrganizationManagerId(organizationManagerId).Count()
                };
            }
            else
            {
                var posts = repository.GetAllPostByOrganizationManagerId(organizationManagerId);
                var postReponses = new List<PostResponse>();

                foreach (var post in posts)
                {
                    var request = _createPostRequestRepository.GetCreatePostRequestByPostId(post.PostID);
                    if (request != null)
                    {
                        request.Post = null;
                        request.OrganizationManager = null;
                        request.Moderator = null;
                        request.Member = null;
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
                        IsDisable = post.IsDisable,
                        Title = post.Title,
                        UpdateAt = post.UpdateAt,
                        AuthorName = om!.FirstName.Trim() + " " + om!.LastName.Trim(),
                        CreatePostRequest = request
                    });
                }
                return new PostCreateByOM
                {
                    Posts = postReponses.Where(p => p.IsDisable == false).ToList(),
                    TotalItem = repository.GetAllPostByOrganizationManagerId(organizationManagerId).Count()
                };
            }

        }

        public PostCreateByVolunteer? GetAllPostsByMemberId(Guid memberId, string? title, int? pageSize, int? pageNo)
        {
            if (!String.IsNullOrEmpty(title))
            {
                var posts = repository.GetAllPostsByMemberId(memberId, pageSize, pageNo);
                var postReponses = new List<PostResponse>();


                foreach (var post in posts)
                {
                    var request = _createPostRequestRepository.GetCreatePostRequestByPostId(post.PostID);
                    if (request != null)
                    {
                        request.Post = null;
                        request.OrganizationManager = null;
                        request.Moderator = null;
                        request.Member = null;
                    }

                    var member = _memberRepository.GetById(memberId);
                    postReponses.Add(new PostResponse
                    {
                        PostID = post.PostID,
                        Content = post.Content,
                        Cover = post.Cover,
                        CreateAt = post.CreateAt,
                        Description = post.Description,
                        Image = post.Image,
                        IsActive = post.IsActive,
                        IsDisable = post.IsDisable,
                        Title = post.Title,
                        UpdateAt = post.UpdateAt,
                        AuthorName = member!.FirstName.Trim() + " " + member!.LastName.Trim(),
                        CreatePostRequest = request
                    });
                }
                return new PostCreateByVolunteer
                {
                    Posts = postReponses.Where(p => p.Title.ToLower().Contains(title.ToLower().Trim()) && p.IsDisable == false).ToList(),
                    TotalItem = repository.GetAllPostsByMemberId(memberId).Count()
                };

            }
            else
            {
                var posts = repository.GetAllPostsByMemberId(memberId);
                var postReponses = new List<PostResponse>();


                foreach (var post in posts)
                {
                    var request = _createPostRequestRepository.GetCreatePostRequestByPostId(post.PostID);
                    if (request != null)
                    {
                        request.Post = null;
                        request.OrganizationManager = null;
                        request.Moderator = null;
                        request.Member = null;
                    }
                    var member = _memberRepository.GetById(memberId);
                    postReponses.Add(new PostResponse
                    {
                        PostID = post.PostID,
                        Content = post.Content,
                        Cover = post.Cover,
                        CreateAt = post.CreateAt,
                        Description = post.Description,
                        Image = post.Image,
                        IsActive = post.IsActive,
                        IsDisable = post.IsDisable,
                        Title = post.Title,
                        UpdateAt = post.UpdateAt,
                        AuthorName = member!.FirstName.Trim() + " " + member!.LastName.Trim(),
                        CreatePostRequest = request
                    });
                }
                return new PostCreateByVolunteer
                {
                    Posts = postReponses.Where(p => p.IsDisable == false).ToList(),
                    TotalItem = repository.GetAllPostsByMemberId(memberId).Count()
                };
            }
        }

        public PostResponse? GetById(Guid id)
        {
            var post = repository.GetById(id);
            if (post == null) { throw new NotFoundException("Bài viết không tìm thấy!"); }
            var postResponse = new PostResponse
            {
                PostID = post.PostID,
                Content = post.Content,
                Cover = post.Cover,
                CreateAt = post.CreateAt,
                Description = post.Description,
                Image = post.Image,
                IsActive = post.IsActive,
                IsDisable = post.IsDisable,
                Title = post.Title,
                UpdateAt = post.UpdateAt
            };
            if (post.CreatePostRequest != null && post.CreatePostRequest.CreateByOM != null)
            {
                var om = _organizationManagerRepository.GetById((Guid)post.CreatePostRequest.CreateByOM);
                postResponse.AuthorName = om!.FirstName.Trim() + " " + om!.LastName.Trim();
                return postResponse;
            }
            else if (post.CreatePostRequest != null && post.CreatePostRequest.CreateByMember != null)
            {
                var member = _memberRepository.GetById((Guid)post.CreatePostRequest.CreateByMember);
                postResponse.AuthorName = member!.FirstName.Trim() + " " + member!.LastName.Trim();
                return postResponse;
            }
            else throw new BadRequestException("Bài viết đã tồn tại nhưng không thể tìm thấy yêu cầu thêm tên tác giả cho bài viết này!");
        }


        public async void UpdateUpdatePostRequest(Guid postId, UpdatePostRequest request)
        {
            var post = repository.GetById(postId);
            if (post != null)
            {
                var postRequest = _createPostRequestRepository.GetCreatePostRequestByPostId(postId);
                if (postRequest != null)
                {
                    if (postRequest.IsApproved) throw new BadRequestException("Bài viết này hiện đã được duyệt, vì vậy mọi thông tin của bài viết này không thể chỉnh sửa!");
                }

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
                if (!String.IsNullOrEmpty(request.Description))
                {
                    post.Description = request.Description;
                }
                repository.Update(post);
            }
        }

        public void UpdatePostStatusRequest(UpdatePostStatusRequest request)
        {
            var post = repository.GetById(request.PostId);
            if (post == null)
            {
                throw new NotFoundException("Không tìm thấy bài viết này!");
            }

            var postRequest = _createPostRequestRepository.GetCreatePostRequestByPostId(request.PostId);
            if (postRequest != null)
            {
                if (postRequest.IsApproved)
                    throw new BadRequestException(
                        "Bài viết này hiện đã được duyệt, vì vậy mọi thông tin của bài viết này không thể chỉnh sửa!");
            }

            if (request.IsDisable)
            {
                post.IsActive = false;
                post.IsDisable = true;
            }
            post.UpdateAt = TimeHelper.GetTime(DateTime.UtcNow);
            repository.Update(post);
        }
    }
}
