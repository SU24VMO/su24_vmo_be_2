using BusinessObject.Models;
using Repository.Interfaces;

namespace SU24_VMO_API_2.Services
{
    public class IPAddressService
    {
        private readonly IIPAddressRepository _ipAddressRepository;

        public IPAddressService(IIPAddressRepository ipAddressRepository)
        {
            _ipAddressRepository = ipAddressRepository;
        }

        public IPAddress? CreateIpAddress(IPAddress ipAddress)
        {
            var ipAddressCreated = _ipAddressRepository.Save(ipAddress);
            return ipAddressCreated;
        }
    }
}
