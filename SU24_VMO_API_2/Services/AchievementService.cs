using BusinessObject.Models;
using Repository.Implements;
using Repository.Interfaces;
using SU24_VMO_API.Constants;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.DTOs.Request.AccountRequest;
using SU24_VMO_API.Supporters.TimeHelper;
using System.Text.RegularExpressions;

namespace SU24_VMO_API.Services
{
    public class AchievementService
    {
        private readonly IAchievementRepository _achievementRepository;
        private readonly IOrganizationRepository _organizationRepository;

        public AchievementService(IAchievementRepository achievementRepository, IOrganizationRepository organizationRepository)
        {
            _achievementRepository = achievementRepository;
            _organizationRepository = organizationRepository;
        }

        public IEnumerable<Achievement> GetAll()
        {
            return _achievementRepository.GetAll();
        }

        public Achievement? GetById(Guid id)
        {
            return _achievementRepository.GetById(id);
        }

        public Achievement? Save(Achievement entity)
        {
            return _achievementRepository.Save(entity);
        }

        public Achievement? CreateAchievement(CreateAchievementRequest request)
        {
            //bắt lỗi validate giá trị truyền vào
            TryValidateCreateAchievementRequest(request);
            //khởi tạo biến và gán giá trị cho từng field của biến này 
            //để lưu giá trị xống database
            var achievement = new Achievement
            {
                AchievementID = Guid.NewGuid(),
                OrganizationID = request.OrganizationID,
                Description = request.Description,
                Link = request.Link,
                Title = request.Title,
                CreatedDate = TimeHelper.GetTime(DateTime.UtcNow)
            };
            //lưu biến achievement vừa tạo xuống db
            var achievementCreated = _achievementRepository.Save(achievement);
            //trả về kết quả vừa tạo
            return achievementCreated;
        }

        public void Update(UpdateAchievementRequest request)
        {
            var achievement = _achievementRepository.GetById(request.AchievementID);
            if (achievement == null) { throw new Exception("Achievement not found!");}
            if (!String.IsNullOrEmpty(request.Description))
            {
                achievement.Description = request.Description;
            }
            if (!String.IsNullOrEmpty(request.Title))
            {
                achievement.Title = request.Title;
            }
            if (!String.IsNullOrEmpty(request.Link))
            {
                achievement.Link = request.Link;
            }

            _achievementRepository.Update(achievement);
        }

        private void TryValidateCreateAchievementRequest(CreateAchievementRequest request)
        {
            if (!String.IsNullOrEmpty(request.OrganizationID.ToString()))
            {
                throw new Exception("OrganizationID is not empty.");
            }

            if (_organizationRepository.GetById(request.OrganizationID) == null)
            {
                throw new Exception("Organization not found.");
            }
            if (!String.IsNullOrEmpty(request.Description))
            {
                throw new Exception("Description is not empty.");
            }
            if (!String.IsNullOrEmpty(request.Title))
            {
                throw new Exception("Title is not empty.");
            }
        }
    }
}
