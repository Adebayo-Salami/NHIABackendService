using IdentityModel;
using NHIABackendService.Core.Utility;
using System;
using System.Linq;
using System.Security.Claims;

namespace NHIABackendService.Core.AspnetCore.Identity
{
    public class UserPrincipal : ClaimsPrincipal
    {
        public UserPrincipal(ClaimsPrincipal principal) : base(principal)
        {
        }

        public string UserName
        {
            get
            {
                if (FindFirst(JwtClaimTypes.Id) == null)
                    return string.Empty;

                return GetClaimValue(JwtClaimTypes.Id);
            }
        }

        public string Email
        {
            get
            {
                if (FindFirst(JwtClaimTypes.Email) == null)
                    return string.Empty;

                return GetClaimValue(JwtClaimTypes.Email);
            }
        }

        public Guid UserId
        {
            get
            {
                if (FindFirst(CoreConstants.UserIdKey) == null)
                    return default;

                if (Guid.TryParse(GetClaimValue(CoreConstants.UserIdKey), out var userId))
                    return userId;

                return Guid.Empty;
            }
        }

        public string EmailAddress
        {
            get
            {
                var emailClaim = FindFirst(Entities.Helpers.ClaimTypesHelper.Email);

                if (emailClaim == null)
                    return string.Empty;

                return emailClaim.Value;
            }
        }

        public Guid ID
        {
            get
            {
                var idClaim = FindFirst(Entities.Helpers.ClaimTypesHelper.ID);

                if (idClaim == null)
                    return Guid.Empty;

                if (!Guid.TryParse(idClaim.Value, out Guid val))
                    return Guid.Empty;

                return Guid.Parse(idClaim.Value);
            }
        }

        public string FirstName
        {
            get
            {
                var usernameClaim = FindFirst(JwtClaimTypes.GivenName);

                if (usernameClaim == null)
                    return string.Empty;

                return usernameClaim.Value;
            }
        }

        public string LastName
        {
            get
            {
                var usernameClaim = FindFirst(JwtClaimTypes.FamilyName);

                if (usernameClaim == null)
                    return string.Empty;

                return usernameClaim.Value;
            }
        }

        private string GetClaimValue(string key)
        {
            var identity = Identity as ClaimsIdentity;
            if (identity == null)
                return null;

            var claim = identity.Claims.FirstOrDefault(c => c.Type == key);
            return claim?.Value;
        }
    }
}
