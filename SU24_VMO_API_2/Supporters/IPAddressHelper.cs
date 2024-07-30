namespace SU24_VMO_API_2.Supporters
{
    public class IpAddressHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IpAddressHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetClientIp()
        {
            var context = _httpContextAccessor.HttpContext;

            if (context != null)
            {
                // If using a proxy server, you might get the real client IP from headers like X-Forwarded-For
                if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
                {
                    return context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                }
                return context.Connection.RemoteIpAddress?.ToString();
            }
            return null;
        }
    }
}
