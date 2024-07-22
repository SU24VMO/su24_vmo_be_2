using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Repository.Implements;
using Repository.Interfaces;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.DTOs.Request.AccountRequest;
using SU24_VMO_API.Supporters.ExceptionSupporter;
using SU24_VMO_API.Supporters.TimeHelper;
using SU24_VMO_API_2.DTOs.Request;

namespace SU24_VMO_API.Services
{
    public class CreateOrganizationRequestService
    {
        private readonly ICreateOrganizationRequestRepository repository;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IOrganizationManagerRepository _organizationManagerRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly FirebaseService _firebaseService;

        public CreateOrganizationRequestService(ICreateOrganizationRequestRepository repository, IOrganizationRepository organizationRepository,
            IOrganizationManagerRepository organizationManagerRepository, INotificationRepository notificationRepository, FirebaseService firebaseService,
            IAccountRepository accountRepository)
        {
            this.repository = repository;
            this._organizationRepository = organizationRepository;
            _organizationManagerRepository = organizationManagerRepository;
            _notificationRepository = notificationRepository;
            _firebaseService = firebaseService;
            _accountRepository = accountRepository;
        }

        public IEnumerable<CreateOrganizationRequest> GetAll()
        {
            var requests = repository.GetAll();
            foreach (var request in requests)
            {
                var organization = _organizationRepository.GetById(request.OrganizationID);
                if (organization != null)
                {
                    organization.CreateOrganizationRequest = null;
                    organization.OrganizationManager = null;
                    organization.Achievements = null;
                    organization.Campaigns = null;
                }
                request.Organization = organization;
            }
            return requests;
        }

        public IEnumerable<CreateOrganizationRequest> GetAllByOrganizationName(string? organizationName)
        {
            if (!String.IsNullOrEmpty(organizationName))
            {
                var requests = repository.GetAll().Where(m => !(String.IsNullOrEmpty(m.OrganizationName)) && m.OrganizationName.ToLower().Contains(organizationName.ToLower().Trim()));
                foreach (var request in requests)
                {
                    var organization = _organizationRepository.GetById(request.OrganizationID);
                    if (organization != null)
                    {
                        organization.CreateOrganizationRequest = null;
                        organization.OrganizationManager = null;
                        organization.Achievements = null;
                        organization.Campaigns = null;
                    }
                    request.Organization = organization;
                }
                return requests;
            }
            else
            {
                var requests = repository.GetAll();
                foreach (var request in requests)
                {
                    var organization = _organizationRepository.GetById(request.OrganizationID);
                    if (organization != null)
                    {
                        organization.CreateOrganizationRequest = null;
                        organization.OrganizationManager = null;
                        organization.Achievements = null;
                        organization.Campaigns = null;
                    }
                    request.Organization = organization;
                }
                return requests;
            }
        }

        public CreateOrganizationRequest? GetById(Guid id)
        {
            return repository.GetById(id);
        }

        public async Task<CreateOrganizationRequest?> CreateOrganizationRequest(Guid organizationManagerID, CreateOrganizationRequestRequest request)
        {
            var organizationExisted = _organizationRepository
                .GetAll()
                .FirstOrDefault(o => o.Tax.Trim().ToLower().Equals(request.OrganizationTaxCode.Trim().ToLower()));
            if (organizationExisted != null)
            {
                throw new BadRequestException("Tổ chức với mã số thuế này đã tồn tại!");
            }


            var organizationManager = _organizationManagerRepository.GetById(organizationManagerID);
            if (organizationManager == null) { throw new NotFoundException("Quản lý tổ chức không tìm thấy!"); }
            var organization = new Organization
            {
                OrganizationID = Guid.NewGuid(),
                OrganizationManagerID = organizationManagerID,
                Name = request.OrganizationName,
                Logo = await _firebaseService.UploadImage(request.Logo!),
                Description = request.PlanInformation,
                Website = request.SocialMediaLink,
                Tax = request.OrganizationTaxCode,
                Location = request.Address,
                FoundingDate = request.FoundingDate,
                OperatingLicense = request.AuthorizationDocuments,
                CreatedAt = TimeHelper.GetTime(DateTime.UtcNow),
                IsActive = false,
                IsModify = false,
                IsDisable = false,
                Category = request.AreaOfActivity
            };

            var createOrganizationRequest = new CreateOrganizationRequest
            {
                CreateOrganizationRequestID = Guid.NewGuid(),
                OrganizationID = organization.OrganizationID,
                OrganizationName = request.OrganizationName,
                AchievementLink = request.AchievementLink,
                Address = request.Address,
                OrganizationManagerEmail = request.OrganizationManagerEmail,
                AreaOfActivity = request.AreaOfActivity,
                AuthorizationDocuments = request.AuthorizationDocuments,
                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                CreateBy = organizationManagerID,
                FoundingDate = request.FoundingDate,
                PlanInformation = request.PlanInformation,
                OrganizationTaxCode = request.OrganizationTaxCode,
                SocialMediaLink = request.SocialMediaLink,
                IsApproved = false,
                IsLocked = false,
                IsPending = true,
                IsRejected = false
            };

            var notification = new Notification
            {
                AccountID = organizationManager.AccountID,
                NotificationID = Guid.NewGuid(),
                NotificationCategory = BusinessObject.Enums.NotificationCategory.SystemMessage,
                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                IsSeen = false,
                Content = "Yêu cầu tạo tổ chức của bạn vừa được tạo thành công, vui lòng đợi hệ thống phản hồi trong giây lát!"
            };
            var organizationCreated = _organizationRepository.Save(organization); if (organizationCreated == null) return null;
            var notificationCreated = _notificationRepository.Save(notification); if (notificationCreated == null) return null;
            var requestCreated = repository.Save(createOrganizationRequest);
            return requestCreated;
        }



        public async Task<bool> UpdateCreateOrganizationRequestRequest(Guid createOrganizationRequestRequestId, UpdateCreateOrganizationRequestRequest updateRequest)
        {
            var requestExisted = repository.GetById(createOrganizationRequestRequestId);
            if (requestExisted == null) throw new NotFoundException("Đơn duyệt tạo tổ chức không tìm thấy!");
            if (requestExisted.IsApproved) throw new BadRequestException("Đơn tạo tổ chức này hiện đã được duyệt, vì vậy mọi thông tin của đơn này hiện không thể chỉnh sửa!");
            var organization = _organizationRepository.GetById(requestExisted.OrganizationID);
            if (organization == null) throw new NotFoundException("Tổ chức không tìm thấy!");

            if (!String.IsNullOrEmpty(updateRequest.Address))
            {
                requestExisted.Address = updateRequest.Address;
                organization.Location = updateRequest.Address;
            }

            if (!String.IsNullOrEmpty(updateRequest.AchievementLink))
            {
                requestExisted.AchievementLink = updateRequest.AchievementLink;
            }
            if (!String.IsNullOrEmpty(updateRequest.AreaOfActivity))
            {
                requestExisted.AreaOfActivity = updateRequest.AreaOfActivity;
            }
            if (!String.IsNullOrEmpty(updateRequest.AuthorizationDocuments))
            {
                requestExisted.AuthorizationDocuments = updateRequest.AuthorizationDocuments;
                organization.OperatingLicense = updateRequest.AuthorizationDocuments;
            }
            if (!String.IsNullOrEmpty(updateRequest.OrganizationManagerEmail))
            {
                requestExisted.OrganizationManagerEmail = updateRequest.OrganizationManagerEmail;
            }
            if (!String.IsNullOrEmpty(updateRequest.OrganizationName))
            {
                requestExisted.OrganizationName = updateRequest.OrganizationName;
                organization.Name = updateRequest.OrganizationName;
            }
            if (!String.IsNullOrEmpty(updateRequest.OrganizationTaxCode))
            {
                requestExisted.OrganizationTaxCode = updateRequest.OrganizationTaxCode;
                organization.Tax = updateRequest.OrganizationTaxCode;
            }
            if (!String.IsNullOrEmpty(updateRequest.PlanInformation))
            {
                requestExisted.PlanInformation = updateRequest.PlanInformation;
                organization.Description = updateRequest.PlanInformation;
            }
            if (!String.IsNullOrEmpty(updateRequest.SocialMediaLink))
            {
                requestExisted.SocialMediaLink = updateRequest.SocialMediaLink;
                organization.Website = updateRequest.SocialMediaLink;
            }
            if (updateRequest.FoundingDate != null)
            {
                requestExisted.FoundingDate = updateRequest.FoundingDate;
                organization.FoundingDate = updateRequest.FoundingDate;
            }

            if (updateRequest.Logo != null)
            {
                organization.Logo = await _firebaseService.UploadImage(updateRequest.Logo);
            }

            _organizationRepository.Update(organization);
            repository.Update(requestExisted);
            return true;
        }


        public bool AcceptOrRejectCreateOrganizationRequest(AcceptOrRejectCreateOrganizationRequestRequest acceptOrRejectCreateOrganizationRequestRequest)
        {
            TryValidateAcceptOrRejectCreateOrganizationRequest(acceptOrRejectCreateOrganizationRequestRequest);
            var request = repository.GetById((Guid)acceptOrRejectCreateOrganizationRequestRequest.CreateOrganizationRequestID!);
            if (request == null) { throw new NotFoundException("Yêu cầu không tìm thấy!"); }
            var organizationManager = _organizationManagerRepository.GetById(request.CreateBy);
            var organization = _organizationRepository.GetById(request.OrganizationID)!;
            var account = _accountRepository.GetById(organizationManager!.AccountID);
            var result = false;
            if (request == null)
            {
                return result;
            }


            var notification = new Notification
            {
                NotificationID = Guid.NewGuid(),
                NotificationCategory = BusinessObject.Enums.NotificationCategory.SystemMessage,
                AccountID = account!.AccountID,
                CreateDate = TimeHelper.GetTime(DateTime.UtcNow),
                IsSeen = false,
            };

            if (acceptOrRejectCreateOrganizationRequestRequest.IsApproved == true)
            {
                request.ApprovedBy = acceptOrRejectCreateOrganizationRequestRequest.ModeratorId;
                request.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);
                request.ApprovedDate = TimeHelper.GetTime(DateTime.UtcNow);
                request.IsApproved = true;
                request.IsPending = false;
                request.IsLocked = false;
                request.IsRejected = false;
                result = true;
                notification.Content = "Yêu cầu tạo tổ chức của bạn vừa được duyệt thành công, hãy trải nghiệm dịch vụ nhé!";
                organization.IsActive = true;
                organization.IsModify = true;
                //organization.IsDisable = false;
            }
            else
            {
                request.UpdateDate = TimeHelper.GetTime(DateTime.UtcNow);
                request.ApprovedBy = acceptOrRejectCreateOrganizationRequestRequest.ModeratorId;
                request.IsApproved = false;
                request.IsPending = false;
                request.IsLocked = false;
                request.IsRejected = true;
                result = true;
                notification.Content = "Yêu cầu tạo tổ chức của bạn chưa được công nhận, hãy cung cấp cho chúng tôi nhiều thông tin xác thực hơn để yêu cầu được dễ dàng thông qua!";
                organization.IsActive = false;
                organization.IsModify = true;
                //organization.IsDisable = true;
            }



            repository.Update(request);
            _organizationRepository.Update(organization);
            _notificationRepository.Save(notification);
            return result;
        }

        private void TryValidateAcceptOrRejectCreateOrganizationRequest(AcceptOrRejectCreateOrganizationRequestRequest request)
        {
            if (String.IsNullOrEmpty(request.CreateOrganizationRequestID.ToString()))
            {
                throw new Exception("Id must not be null or empty!");
            }
            if (String.IsNullOrEmpty(request.IsApproved.ToString()))
            {
                throw new Exception("Trạng thái không được để trống!");
            }
        }


        public void UpdateCreateOrganizationRequest(UpdateCreateOrganizationRequest request)
        {
            TryValidateUpdateOrganizationRequest(request);
            var createOrganizationRequest = repository.GetById(request.CreateOrganizationRequestID)!;
            if (!String.IsNullOrEmpty(request.OrganizationName))
            {
                createOrganizationRequest.OrganizationName = request.OrganizationName;
            }

            if (!String.IsNullOrEmpty(request.Address))
            {
                createOrganizationRequest.Address = request.Address;
            }
            if (!String.IsNullOrEmpty(request.OrganizationManagerEmail))
            {
                createOrganizationRequest.OrganizationManagerEmail = request.OrganizationManagerEmail;
            }
            if (!String.IsNullOrEmpty(request.OrganizationTaxCode))
            {
                createOrganizationRequest.OrganizationTaxCode = request.OrganizationTaxCode;
            }
            if (!String.IsNullOrEmpty(request.FoundingDate.ToString()))
            {
                createOrganizationRequest.FoundingDate = (DateTime)request.FoundingDate!;
            }
            if (!String.IsNullOrEmpty(request.SocialMediaLink))
            {
                createOrganizationRequest.SocialMediaLink = request.SocialMediaLink!;
            }
            if (!String.IsNullOrEmpty(request.AreaOfActivity))
            {
                createOrganizationRequest.AreaOfActivity = request.AreaOfActivity!;
            }
            if (!String.IsNullOrEmpty(request.Address))
            {
                createOrganizationRequest.Address = request.Address!;
            }
            if (!String.IsNullOrEmpty(request.PlanInformation))
            {
                createOrganizationRequest.PlanInformation = request.PlanInformation!;
            }
            if (!String.IsNullOrEmpty(request.AchievementLink))
            {
                createOrganizationRequest.AchievementLink = request.AchievementLink!;
            }

            if (!String.IsNullOrEmpty(request.AuthorizationDocuments))
            {
                createOrganizationRequest.AuthorizationDocuments = request.AuthorizationDocuments!;
            }
            repository.Update(createOrganizationRequest);
        }

        //private void TryValidateCreateOrganizationRequest(CreateOrganizationRequestRequest request)
        //{
        //    if (String.IsNullOrEmpty(request.OrganizationID.ToString()))
        //    {
        //        throw new Exception("Id must not be null or empty!");
        //    }
        //}

        private void TryValidateUpdateOrganizationRequest(UpdateCreateOrganizationRequest request)
        {
            if (String.IsNullOrEmpty(request.CreateOrganizationRequestID.ToString()))
            {
                throw new Exception("Id must not be null or empty!");
            }
            if (repository.GetById(request.CreateOrganizationRequestID) == null)
            {
                throw new Exception("Yêu cầu không tồn tại!");
            }
        }
    }
}
