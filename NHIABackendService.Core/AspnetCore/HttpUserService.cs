using System.Linq;
using Microsoft.AspNetCore.Http;
using NHIABackendService.Core.AspnetCore.Identity;
using NHIABackendService.Core.Permissions;

namespace NHIABackendService.Core.AspnetCore
{
    public class HttpUserService : IHttpUserService
    {
        private readonly IHttpContextAccessor _httpContext;

        public HttpUserService(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }

        public bool CheckCurrentUserHasPermission(Permission permission)
        {
            var user = GetCurrentUser();
            var checkUserHasPermission = user?.Claims
                .Where(x => x.Type.Equals(nameof(Permission)))
                .Any(x => x.Value == permission.ToString());

            return checkUserHasPermission ?? false;
        }

        public UserPrincipal GetCurrentUser()
        {
            if (_httpContext.HttpContext?.User != null) return new UserPrincipal(_httpContext.HttpContext.User);

            return null;
        }
    }
}
