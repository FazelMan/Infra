using System;
using System.Linq;
using System.Security.Claims;
using Infra.Shared.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace Infra.Shared.Http.Extensions
{
    public static class HttpContextExtensions
    {
        public static string GetUserId(this IHttpContextAccessor httpContextAccessor)
        {
            if (!httpContextAccessor.HttpContext.User.Identity.IsAuthenticated) 
                return null;
            
            var identity = httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;
            var userId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return !string.IsNullOrWhiteSpace(userId) ? userId : null;
        } 
        
        public static string[] GetUserRoles(this IHttpContextAccessor httpContextAccessor)
        {
            if (!httpContextAccessor.HttpContext.User.Identity.IsAuthenticated) 
                return null;
            
            var identity = httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;
            var roles = httpContextAccessor.HttpContext.User.FindAll(ClaimTypes.Role);
            return roles.Select(x=>x.Value).ToArray();
        }
    }
}