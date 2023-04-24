using Infra.Shared.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Infra.Shared.Extensions
{
    public static class HttpRequestExtensions
    {
        public static string GetBaseAddress(this HttpRequest request)
        {
            return $"{request.Scheme}://{request.Host}{request.PathBase}";
        }

        public static string GetBearerToken(this HttpRequest request, bool throwError = true)
        {
            string token = null;

            var authHeader = request.Headers["authorization"].ToString();

            if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.Length > 7)
                token = authHeader.Substring(7);
            else if (throwError)
                throw new UnauthorizedException("Authorization token not set");

            return token;
        }
    }
}
