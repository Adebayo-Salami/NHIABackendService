using NHIABackendService.Core.Permissions;

namespace NHIABackendService.Core.AspnetCore
{
    public interface IHttpUserService
    {
        bool CheckCurrentUserHasPermission(Permission permission);
    }
}
