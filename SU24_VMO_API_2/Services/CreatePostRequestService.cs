using BusinessObject.Models;
using Google.Api.Gax.ResourceNames;
using Repository.Implements;
using Repository.Interfaces;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.Supporters.ExceptionSupporter;
using SU24_VMO_API.Supporters.TimeHelper;
using SU24_VMO_API_2.DTOs.Request;
using System.Security.Principal;
using BusinessObject.Enums;

namespace SU24_VMO_API.Services
{
    public class CreatePostRequestService
    {
        private readonly ICreatePostRequestRepository _repository;
        private readonly IOrganizationManagerRepository _organizationManagerRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IModeratorRepository _moderatorRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IPostRepository _postRepository;
        private readonly FirebaseService _firebaseService;

        public CreatePostRequestService(ICreatePostRequestRepository repository, IAccountRepository accountRepository,
            IMemberRepository memberRepository, FirebaseService firebaseService, IPostRepository postRepository,
            IModeratorRepository moderatorRepository, IOrganizationManagerRepository organizationManagerRepository,
            INotificationRepository notificationRepository)
        {
            _repository = repository;
            _accountRepository = accountRepository;
            _memberRepository = memberRepository;
            _firebaseService = firebaseService;
            _postRepository = postRepository;
            _moderatorRepository = moderatorRepository;
            _organizationManagerRepository = organizationManagerRepository;
            _notificationRepository = notificationRepository;
        }

        public IEnumerable<CreatePostRequest> GetAll()
        {
            return _repository.GetAll();
        }

        public IEnumerable<CreatePostRequest> GetAllByPostName(string? postTitle)
        {
            if (!String.IsNullOrEmpty(postTitle))
                return _repository.GetAll().Where(m => m.Post.Title.Trim().ToLower().Contains(postTitle.ToLower().Trim()));
            else return _repository.GetAll();
        }

        public CreatePostRequest? GetById(Guid id)
        {
            return _repository.GetById(id);
        }

        public async Task<CreatePostRequest?> CreateCreatePostRequest(CreatePostRequestRequest request)
        {
            var account = _accountRepository.GetById(request.AccountId);
            //them post trc
            var post = new Post
            {
                PostID = Guid.NewGuid(),
                Cover = await _firebaseService.UploadImage(request.Cover),
                Title = request.Title,
                Content = request.Content,
                IsActive = false,
                IsDisable = false,
                Image = await _firebaseService.UploadImage(request.Image),
                Description = request.Description,
                CreateAt = TimeHelper.GetTime(DateTime.UtcNow),
            };
            var postCreated = _postRepository.Save(post);
            if (postCreated == null) { throw new Exception("Tạo đơn thất bại!"); }

            //them request tao bai post sau

            var notification = new Notification
            {
                NotificationID = Guid.NewGuid(),
                NotificationCategory = BusinessObject.Enums.NotificationCategory.SystemMessage,
                AccountID = account!.AccountID,
                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                IsSeen = false,
                Content = "Yêu cầu tạo bài viết của bạn vừa được tạo thành công, vui lòng đợi hệ thống phản hồi trong giây lát!"
            };


            if (account == null) { throw new NotFoundException("Tài khoản không tìm thấy!"); }
            if (account.Role == BusinessObject.Enums.Role.Volunteer)
            {
                var volunteer = _memberRepository.GetByAccountId(request.AccountId)!;
                var createPostRequest = new CreatePostRequest
                {
                    CreatePostRequestID = Guid.NewGuid(),
                    PostID = postCreated.PostID,
                    CreateByMember = volunteer.MemberID,
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
                throw new UnauthorizedAccessException("Người dùng hiện tại không thể thực hiện chức năng này!");
            }

        }



        public async Task<bool> UpdateCreatePostRequestRequest(Guid createPostRequestId, UpdateCreatePostRequestRequest updateRequest)
        {
            var requestExisted = _repository.GetById(createPostRequestId);
            if (requestExisted == null)
            {
                throw new NotFoundException("Đơn tạo này không tìm thấy!");
            }

            if (requestExisted.IsApproved) throw new BadRequestException("Đơn tạo bài viết này hiện đã được duyệt, vì vậy mọi thông tin của đơn này hiện không thể chỉnh sửa!");

            var post = _postRepository.GetById(requestExisted.PostID);
            if (post == null) throw new NotFoundException("Không tìm thấy bài viết này!");
            if (!String.IsNullOrEmpty(updateRequest.Description))
            {
                post.Description = updateRequest.Description;
            }
            if (!String.IsNullOrEmpty(updateRequest.Content))
            {
                post.Content = updateRequest.Content;
            }
            if (!String.IsNullOrEmpty(updateRequest.Title))
            {
                post.Title = updateRequest.Title;
            }

            if (updateRequest.Image != null)
            {
                post.Image = await _firebaseService.UploadImage(updateRequest.Image);
            }
            if (updateRequest.Cover != null)
            {
                post.Cover = await _firebaseService.UploadImage(updateRequest.Cover);
            }

            _postRepository.Update(post);
            return true;
        }



        public void AcceptOrRejectCreatePostRequest(UpdateCreatePostRequest request)
        {
            var createPostRequest = _repository.GetById(request.CreatePostRequestId);
            if (createPostRequest == null) { throw new NotFoundException("Đơn tạo bài viết này không tìm thấy!"); }
            var moderator = _moderatorRepository.GetById(request.ModeratorId);
            if (moderator == null) { throw new NotFoundException("Kiểm duyệt viên không tìm thấy!"); }

            var om = new OrganizationManager();
            var user = new Member();

            var notification = new Notification
            {
                NotificationID = Guid.NewGuid(),
                NotificationCategory = BusinessObject.Enums.NotificationCategory.SystemMessage,
                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                IsSeen = false,
            };

            if (createPostRequest != null && moderator != null)
            {
                var post = _postRepository.GetById(createPostRequest.PostID)!;
                if (request.IsApproved)
                {
                    createPostRequest.ApprovedBy = request.ModeratorId;
                    createPostRequest.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);
                    createPostRequest.ApprovedDate = TimeHelper.GetTime(DateTime.UtcNow);
                    createPostRequest.IsApproved = true;
                    createPostRequest.IsLocked = false;
                    createPostRequest.IsPending = false;
                    createPostRequest.IsRejected = false;
                    post.IsActive = true;
                    //post.IsDisable = false;
                    if (createPostRequest.CreateByOM != null)
                    {
                        om = _organizationManagerRepository.GetById((Guid)createPostRequest.CreateByOM)!;
                        notification.AccountID = om.AccountID;
                        notification.Content = "Yêu cầu tạo bài viết của bạn vừa được duyệt thành công, hãy trải nghiệm dịch vụ nhé!";
                    }
                    else
                    {
                        user = _memberRepository.GetById((Guid)createPostRequest.CreateByMember!)!;
                        notification.AccountID = user.AccountID;
                        notification.Content = "Yêu cầu tạo bài viết của bạn vừa được duyệt thành công, hãy trải nghiệm dịch vụ nhé!";
                    }
                }
                else
                {
                    createPostRequest.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);
                    createPostRequest.ApprovedBy = request.ModeratorId;
                    createPostRequest.IsApproved = false;
                    createPostRequest.IsLocked = false;
                    createPostRequest.IsPending = false;
                    createPostRequest.IsRejected = true;
                    post.IsActive = false;
                    //post.IsDisable = true;
                    if (createPostRequest.CreateByOM != null)
                    {
                        om = _organizationManagerRepository.GetById((Guid)createPostRequest.CreateByOM)!;
                        notification.AccountID = om.AccountID;
                        notification.Content = "Yêu cầu tạo bài viết của bạn chưa được công nhận, hãy kiểm tra kĩ hơn để yêu cầu được dễ dàng thông qua!";
                    }
                    else
                    {
                        user = _memberRepository.GetById((Guid)createPostRequest.CreateByMember!)!;
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
