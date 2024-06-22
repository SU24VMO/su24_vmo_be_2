using BusinessObject.Models;
using Repository.Implements;
using Repository.Interfaces;
using SU24_VMO_API.DTOs.Request;
using SU24_VMO_API.DTOs.Request.AccountRequest;
using SU24_VMO_API.Supporters.TimeHelper;

namespace SU24_VMO_API.Services
{
    public class CampaignTypeService
    {
        private readonly ICampaignTypeRepository _campaignTypeRepository;

        public CampaignTypeService(ICampaignTypeRepository campaignTypeRepository)
        {
            _campaignTypeRepository = campaignTypeRepository;
        }

        public IEnumerable<CampaignType>? GetCampaignTypes()
        {
            return _campaignTypeRepository.GetAll();
        }

        public CampaignType? GetCampaignTypeById(Guid campaignTypeId)
        {
            return _campaignTypeRepository.GetById(campaignTypeId);
        }

        public CampaignType? CreateCampaignType (CreateCampaignTypeRequest campaignType)
        {
            var campaignCreated = _campaignTypeRepository.Save(new CampaignType
            {
                CampaignTypeID = Guid.NewGuid(),
                Name = campaignType.Name,
                CreateAt = TimeHelper.GetTime(DateTime.UtcNow),
                IsValid = true,
            });
            return campaignCreated;
        }

        public void UpdateCampaignType(UpdateCampaignTypeRequest request)
        {
            TryValidateUpdateCampaignTypeRequest(request);
            var campaignType = _campaignTypeRepository.GetById(request.CampaignTypeID)!;
            if(!String.IsNullOrEmpty(request.Name)) campaignType.Name = request.Name;
            if(request.IsValid != null) campaignType.IsValid = (bool)request.IsValid;

            _campaignTypeRepository.Update(campaignType);
        }



        private void TryValidateUpdateCampaignTypeRequest(UpdateCampaignTypeRequest request)
        {
            if (!String.IsNullOrEmpty(request.CampaignTypeID.ToString()))
            {
                throw new Exception("CampaignTypeID must not empty.");
            }

            if (_campaignTypeRepository.GetById(request.CampaignTypeID) == null)
            {
                throw new Exception("CampaignType not found.");
            }
        }
    }
}
