using Microsoft.AspNetCore.Http.Features;
using SU24_VMO_API.Attributes;
using SU24_VMO_API.Services;

namespace SU24_VMO_API.Supporters.Middlewares
{
    public class DbTransactionMiddleware
    {
        private readonly RequestDelegate _next;
        private DBTransactionService _dbTransactionService;

        public DbTransactionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, DBTransactionService dbTransactionService)
        {
            _dbTransactionService = dbTransactionService;
            // For HTTP GET opening transaction is not required
            if (httpContext.Request.Method.Equals("GET", StringComparison.CurrentCultureIgnoreCase))
            {
                await _next(httpContext);
                return;
            }

            // If action is not decorated with TransactionAttribute then skip opening transaction
            var endpoint = httpContext.Features.Get<IEndpointFeature>()?.Endpoint;
            var attribute = endpoint?.Metadata.GetMetadata<DBTransactionAttribute>();
            if (attribute == null)
            {
                await _next(httpContext);
                return;
            }


            try
            {
                await _dbTransactionService.CreateTransactionAsync();

                await _next(httpContext);
            }
            finally
            {
                if (_dbTransactionService.IsExist())
                    await _dbTransactionService.DisposeAsync();
            }
        }
    }
}
