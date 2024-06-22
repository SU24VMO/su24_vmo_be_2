using BusinessObject.Models;
using Repository.Implements;
using Repository.Interfaces;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.Supporters.ExceptionSupporter;
using SU24_VMO_API.Supporters.TimeHelper;

namespace SU24_VMO_API.Services
{
    public class CreatePostRequestService
    {
        private readonly ICreatePostRequestRepository _repository;
        private readonly IOrganizationManagerRepository _organizationManagerRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IRequestManagerRepository _requestManagerRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPostRepository _postRepository;
        private readonly FirebaseService _firebaseService;

        public CreatePostRequestService(ICreatePostRequestRepository repository, IAccountRepository accountRepository,
            IUserRepository userRepository, FirebaseService firebaseService, IPostRepository postRepository,
            IRequestManagerRepository requestManagerRepository, IOrganizationManagerRepository organizationManagerRepository)
        {
            _repository = repository;
            _accountRepository = accountRepository;
            _userRepository = userRepository;
            _firebaseService = firebaseService;
            _postRepository = postRepository;
            _requestManagerRepository = requestManagerRepository;
            _organizationManagerRepository = organizationManagerRepository;
        }

        public IEnumerable<CreatePostRequest> GetAll()
        {
            return _repository.GetAll();
        }

        public CreatePostRequest? GetById(Guid id)
        {
            return _repository.GetById(id);
        }

        public async Task<CreatePostRequest?> CreateCreatePostRequest(CreatePostRequestRequest request)
        {
            //them post trc
            var post = new Post
            {
                PostID = Guid.NewGuid(),
                Cover = await _firebaseService.UploadImage(request.Cover),
                Title = request.Title,
                Content = request.Content,
                Image = await _firebaseService.UploadImage(request.Image),
                CreateAt = TimeHelper.GetTime(DateTime.UtcNow),
            };
            var postCreated = _postRepository.Save(post);
            if (postCreated != null) { return null; }

            //them request tao bai post sau

            var account = _accountRepository.GetById(request.AccountId);
            if (account == null) { throw new NotFoundException("Account not found!"); }
            if (account.Role == BusinessObject.Enums.Role.Member)
            {
                var user = _userRepository.GetByAccountId(request.AccountId)!;
                var createPostRequest = new CreatePostRequest
                {
                    CreatePostRequestID = Guid.NewGuid(),
                    PostID = post.PostID,
                    CreateByUser = user.UserID,
                    CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                    IsApproved = false,
                    IsLocked = false,
                    IsPending = true,
                    IsRejected = false
                };
                var createPostRequestCreated = _repository.Save(createPostRequest);
                return createPostRequestCreated;
            }
            else if (account.Role == BusinessObject.Enums.Role.OrganizationManager)
            {
                var organizationManager = _organizationManagerRepository.GetByAccountID(request.AccountId)!;
                var createPostRequest = new CreatePostRequest
                {
                    CreatePostRequestID = Guid.NewGuid(),
                    PostID = post.PostID,
                    CreateByOM = organizationManager.OrganizationManagerID,
                    CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                    IsApproved = false,
                    IsLocked = false,
                    IsPending = true,
                    IsRejected = false
                };
                var createPostRequestCreated = _repository.Save(createPostRequest);
                return createPostRequestCreated;
            }
            else
            {
                return null;
            }

        }

        public void AcceptOrRejectCreatePostRequest(UpdateCreatePostRequest request)
        {
            ////them post trc
            //var createPostRequest = _repository.GetById(request.CreatePostRequestId);
            ////them request tao bai post sau
            //if(createPostRequest != null)
            //{
            //    var requestManager = _requestManagerRepository.GetById(request.RequestManagerId);
            //    if (requestManager == null) { throw new Exception("RequestManager not found!"); }

            //    if (!String.IsNullOrEmpty(request.Content))
            //    {

            //    }
            //    if (!String.IsNullOrEmpty(request.Title))
            //    {

            //    }
            //    if (request.Cover != null)
            //    {

            //    }
            //    if (request.Image != null)
            //    {

            //    }
            //}
            var createPostRequest = _repository.GetById(request.CreatePostRequestId);
            var requestManager = _requestManagerRepository.GetById(request.RequestManagerId);
            if (createPostRequest != null && requestManager != null)
            {
                if (request.IsAccept)
                {
                    createPostRequest.ApprovedBy = request.RequestManagerId;
                    createPostRequest.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);
                    createPostRequest.ApprovedDate = TimeHelper.GetTime(DateTime.UtcNow);
                    createPostRequest.IsApproved = true;
                    createPostRequest.IsLocked = false;
                    createPostRequest.IsPending = false;
                    createPostRequest.IsRejected = false;
                }
                else
                {
                    createPostRequest.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);
                    createPostRequest.IsApproved = false;
                    createPostRequest.IsLocked = false;
                    createPostRequest.IsPending = false;
                    createPostRequest.IsRejected = true;
                }
            }

        }
    }
}
