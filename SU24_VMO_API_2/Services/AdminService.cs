using BusinessObject.Models;
using Repository.Interfaces;

namespace SU24_VMO_API.Services
{
    public class AdminService
    {
        private readonly IAdminRepository _adminRepository;

        public AdminService(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public IEnumerable<Admin> GetAll()
        {
            return _adminRepository.GetAll();
        }

        public Admin? GetById(Guid id)
        {
            return _adminRepository.GetById(id);
        }

        public Admin? Save(Admin entity)
        {
            return _adminRepository.Save(entity);
        }

        public void Update(Admin entity)
        {
            _adminRepository.Update(entity);
        }
    }
}
