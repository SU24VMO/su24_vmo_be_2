using BusinessObject.Models;
using Repository.Implements;
using Repository.Interfaces;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.Supporters.ExceptionSupporter;
using SU24_VMO_API.Supporters.TimeHelper;
using System.Security.Principal;

namespace SU24_VMO_API.Services
{
    public class CreatePostRequestService
    {
        private readonly ICreatePostRequestRepository _repository;
        private readonly IOrganizationManagerRepository _organizationManagerRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IRequestManagerRepository _requestManagerRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPostRepository _postRepository;
        private readonly FirebaseService _firebaseService;

        public CreatePostRequestService(ICreatePostRequestRepository repository, IAccountRepository accountRepository,
            IUserRepository userRepository, FirebaseService firebaseService, IPostRepository postRepository,
            IRequestManagerRepository requestManagerRepository, IOrganizationManagerRepository organizationManagerRepository,
            INotificationRepository notificationRepository)
        {
            _repository = repository;
            _accountRepository = accountRepository;
            _userRepository = userRepository;
            _firebaseService = firebaseService;
            _postRepository = postRepository;
            _requestManagerRepository = requestManagerRepository;
            _organizationManagerRepository = organizationManagerRepository;
            _notificationRepository = notificationRepository;
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
                IsActive = false,
                Image = await _firebaseService.UploadImage(request.Image),
                CreateAt = TimeHelper.GetTime(DateTime.UtcNow),
            };
            var postCreated = _postRepository.Save(post);
            if (postCreated == null) { throw new Exception("Create post request fail!"); }

            //them request tao bai post sau

            var account = _accountRepository.GetById(request.AccountId);

            var notification = new Notification
            {
                NotificationID = Guid.NewGuid(),
                NotificationCategory = BusinessObject.Enums.NotificationCategory.SystemMessage,
                AccountID = account!.AccountID,
                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                IsSeen = false,
                Content = "Yêu cầu tạo bài viết của bạn vừa được tạo thành công, vui lòng đợi hệ thống phản hồi trong giây lát!"
            };


            if (account == null) { throw new NotFoundException("Account not found!"); }
            if (account.Role == BusinessObject.Enums.Role.Member)
            {
                var user = _userRepository.GetByAccountId(request.AccountId)!;
                var createPostRequest = new CreatePostRequest
                {
                    CreatePostRequestID = Guid.NewGuid(),
                    PostID = postCreated.PostID,
                    CreateByUser = user.UserID,
                    CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                    IsApproved = false,
                    IsLocked = false,
                    IsPending = true,
                    IsRejected = false
                };
                var createPostRequestCreated = _repository.Save(createPostRequest);
                _notificationRepository.Save(notification);
                return createPostRequestCreated;
            }
            else if (account.Role == BusinessObject.Enums.Role.OrganizationManager)
            {
                var organizationManager = _organizationManagerRepository.GetByAccountID(request.AccountId)!;
                var createPostRequest = new CreatePostRequest
                {
                    CreatePostRequestID = Guid.NewGuid(),
                    PostID = postCreated.PostID,
                    CreateByOM = organizationManager.OrganizationManagerID,
                    CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                    IsApproved = false,
                    IsLocked = false,
                    IsPending = true,
                    IsRejected = false
                };
                var createPostRequestCreated = _repository.Save(createPostRequest);
                _notificationRepository.Save(notification);
                return createPostRequestCreated;
            }
            else
            {
                throw new UnauthorizedAccessException("Role of this account is not accept!");
            }

        }

        public void AcceptOrRejectCreatePostRequest(UpdateCreatePostRequest request)
        {
            var createPostRequest = _repository.GetById(request.CreatePostRequestId);
            var requestManager = _requestManagerRepository.GetById(request.RequestManagerId);
            var om = new OrganizationManager();
            var user = new User();

            var notification = new Notification
            {
                NotificationID = Guid.NewGuid(),
                NotificationCategory = BusinessObject.Enums.NotificationCategory.SystemMessage,
                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                IsSeen = false,
            };

            if (createPostRequest != null && requestManager != null)
            {
                var post = _postRepository.GetById(createPostRequest.PostID)!;
                if (request.IsAccept)
                {
                    createPostRequest.ApprovedBy = request.RequestManagerId;
                    createPostRequest.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);
                    createPostRequest.ApprovedDate = TimeHelper.GetTime(DateTime.UtcNow);
                    createPostRequest.IsApproved = true;
                    createPostRequest.IsLocked = false;
                    createPostRequest.IsPending = false;
                    createPostRequest.IsRejected = false;
                    post.IsActive = true;
                    if (createPostRequest.CreateByOM != null)
                    {
                        om = _organizationManagerRepository.GetById((Guid)createPostRequest.CreateByOM)!;
                        notification.AccountID = om.AccountID;
                        notification.Content = "Yêu cầu tạo bài viết của bạn vừa được duyệt thành công, hãy trải nghiệm dịch vụ nhé!";
                    }
                    else
                    {
                        user = _userRepository.GetById((Guid)createPostRequest.CreateByUser!)!;
                        notification.AccountID = user.AccountID;
                        notification.Content = "Yêu cầu tạo bài viết của bạn vừa được duyệt thành công, hãy trải nghiệm dịch vụ nhé!";
                    }
                }
                else
                {
                    createPostRequest.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);
                    createPostRequest.IsApproved = false;
                    createPostRequest.IsLocked = false;
                    createPostRequest.IsPending = false;
                    createPostRequest.IsRejected = true;
                    post.IsActive = false;
                    if (createPostRequest.CreateByOM != null)
                    {
                        om = _organizationManagerRepository.GetById((Guid)createPostRequest.CreateByOM)!;
                        notification.AccountID = om.AccountID;
                        notification.Content = "Yêu cầu tạo bài viết của bạn chưa được công nhận, hãy kiểm tra kĩ hơn để yêu cầu được dễ dàng thông qua!";
                    }
                    else
                    {
                        user = _userRepository.GetById((Guid)createPostRequest.CreateByUser!)!;
                        notification.AccountID = user.AccountID;
                        notification.Content = "Yêu cầu tạo bài viết của bạn chưa được công nhận, hãy kiểm tra kĩ hơn để yêu cầu được dễ dàng thông qua!";
                    }
                }
                _repository.Update(createPostRequest);
                _postRepository.Update(post);
                _notificationRepository.Save(notification);
            }

        }
    }
}
