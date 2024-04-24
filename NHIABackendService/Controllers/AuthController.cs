using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NHIABackendService.Core.AspnetCore;
using NHIABackendService.Core.ViewModels.Enums;
using NHIABackendService.Core.ViewModels;
using NHIABackendService.Entities;
using NHIABackendService.Services.Interface;
using Microsoft.AspNetCore.Authentication;
using NHIABackendService.Entities.Helpers;
using System.Security.Claims;
using OpenIddict.Abstractions;
using Microsoft.AspNetCore.Authorization;
using NHIABackendService.Core.Extensions;
using OpenIddict.Server.AspNetCore;
using OpenIddict.Core;
using NHIABackendService.Core.Permissions;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace NHIABackendService.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]/[action]")]
    public class AuthController : BaseController
    {
        private readonly IUserService _userService;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IOptions<IdentityOptions> _identityOptions;
        public AuthController(IUserService userService, SignInManager<User> signInManager, UserManager<User> userManager, IOptions<IdentityOptions> identityOptions)
        {
            _userService = userService;
            _signInManager = signInManager;
            _userManager = userManager;
            _identityOptions = identityOptions;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<User>), 200)]
        public async Task<IActionResult> GetUserProfile()
        {
            try
            {
                //var user = await _userManager.FindByIdAsync(CurrentUser.UserId.ToString());
                var user = await _userManager.FindByIdAsync(CurrentUser.ID.ToString());
                var rsp = new ApiResponse<User>
                {
                    Code = ApiResponseCodes.OK,
                    Description = "User profile loaded ok",
                    Payload = user
                };
                return ApiResponse(rsp);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        [AllowAnonymous]
        [HttpPost("~/api/connect/token"), Produces("application/json")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> Token([FromForm] OpenIddictRequest request)
        {
            try
            {
                request.GrantType = "password";
                if (request.IsPasswordGrantType())
                {
                    var user = await _userManager.FindByEmailAsync(request.Username);
                    if (user == null)
                    {
                        return BadRequest(new OpenIddictResponse
                        {
                            Error = OpenIddictConstants.Errors.InvalidGrant,
                            ErrorDescription = "Email or password is incorrect."
                        });
                    }
                    if (user.IsDeleted)
                    {
                        return BadRequest(new OpenIddictResponse
                        {
                            Error = OpenIddictConstants.Errors.InvalidGrant,
                            ErrorDescription = "You are not allowed to sign in."
                        });
                    }
                    // Ensure the user is allowed to sign in.
                    //if (!await _signInManager.CanSignInAsync(user))       //Commented out till we confirm if we are making use of email and phone number confirmation... this method requires that to return true
                    //{
                    //    return BadRequest(new OpenIddictResponse
                    //    {
                    //        Error = OpenIddictConstants.Errors.AccessDenied,
                    //        ErrorDescription = "You are not allowed to sign in."
                    //    });
                    //}
                    // Ensure the user is not already locked out.
                    if (_userManager.SupportsUserLockout && await _userManager.IsLockedOutAsync(user))
                    {
                        var lockoutMinutes = _userManager.Options.Lockout.DefaultLockoutTimeSpan.TotalMinutes;

                        return BadRequest(new OpenIddictResponse
                        {
                            Error = OpenIddictConstants.Errors.AccessDenied,
                            ErrorDescription = $"Your account has been locked. " +
                            $"Please contact the platform Administrators immediately or wait for {lockoutMinutes} minute(s) to retry"
                        });
                    }
                    // Ensure the password is valid.
                    if (!await _userManager.CheckPasswordAsync(user, request.Password))
                    {
                        if (_userManager.SupportsUserLockout)
                        {
                            await _userManager.AccessFailedAsync(user);
                        }

                        return BadRequest(new OpenIddictResponse
                        {
                            Error = OpenIddictConstants.Errors.InvalidGrant,
                            ErrorDescription = "Email or password is incorrect."
                        });
                    }
                    if (_userManager.SupportsUserLockout)
                    {
                        await _userManager.ResetAccessFailedCountAsync(user);
                    }
                    user.LastLoginDate = DateTime.Now;
                    await _userManager.UpdateAsync(user);
                    // Create a new authentication ticket.
                    var ticket = await CreateTicketAsync(request, user);
                    return SignIn(ticket.Principal, ticket.Properties, ticket.AuthenticationScheme);
                }
                else if (request.IsRefreshTokenGrantType())
                {
                    var info = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                    var user = await _userManager.GetUserAsync(info.Principal);
                    if (user == null)
                    {
                        return BadRequest(new OpenIddictResponse
                        {
                            Error = OpenIddictConstants.Errors.InvalidGrant,
                            ErrorDescription = "The refresh token is no longer valid."
                        });
                    }
                    if (!await _signInManager.CanSignInAsync(user))
                    {
                        return BadRequest(new OpenIddictResponse
                        {
                            Error = OpenIddictConstants.Errors.InvalidGrant,
                            ErrorDescription = "The user is no longer allowed to sign in."
                        });
                    }
                    var ticket = await CreateTicketAsync(request, user, info.Properties);
                    return SignIn(ticket.Principal, ticket.Properties, ticket.AuthenticationScheme);
                }

                return BadRequest(new OpenIddictResponse
                {
                    Error = OpenIddictConstants.Errors.UnsupportedGrantType,
                    ErrorDescription = "The specified grant type is not supported."
                });
            }
            catch (Exception error)
            {
                return HandleError(error);
            }
        }

        private async Task<AuthenticationTicket> CreateTicketAsync(OpenIddictRequest oidcRequest, User user, AuthenticationProperties properties = null)
        {
            var principal = await _signInManager.CreateUserPrincipalAsync(user);
            var identity = (ClaimsIdentity)principal.Identity;
            AddUserClaims(user, identity);

            // Create a new authentication ticket holding the user identity.
            var ticket = new AuthenticationTicket(principal, properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            if (!oidcRequest.IsRefreshTokenGrantType())
            {
                // Set the list of scopes granted to the client application.
                // Note: the offline_access scope must be granted
                // to allow OpenIddict to return a refresh token.
                ticket.Principal.SetScopes(new[]
                {
                    OpenIddictConstants.Scopes.OpenId,
                    OpenIddictConstants.Scopes.Email,
                    OpenIddictConstants.Scopes.Profile,
                    OpenIddictConstants.Scopes.OfflineAccess,
                    OpenIddictConstants.Scopes.Roles,

                }.Intersect(oidcRequest.GetScopes()));
            }
            ticket.Principal.SetResources("resource_server");

            var destinations = new List<string>
            {
                OpenIddictConstants.Destinations.AccessToken
            };

            foreach (var claim in ticket.Principal.Claims)
            {
                // Never include the security stamp in the access and identity tokens, as it's a secret value.
                if (claim.Type == _identityOptions.Value.ClaimsIdentity.SecurityStampClaimType)
                {
                    continue;
                }

                // Only add the iterated claim to the id_token if the corresponding scope was granted to the client application.
                // The other claims will only be added to the access_token, which is encrypted when using the default format.
                if ((claim.Type == OpenIddictConstants.Claims.Name && ticket.Principal.HasScope(OpenIddictConstants.Scopes.Profile)) ||
                    (claim.Type == OpenIddictConstants.Claims.Email && ticket.Principal.HasScope(OpenIddictConstants.Scopes.Email)) ||
                    (claim.Type == OpenIddictConstants.Claims.Role && ticket.Principal.HasScope(OpenIddictConstants.Claims.Role)) ||
                    (claim.Type == OpenIddictConstants.Claims.Audience && ticket.Principal.HasScope(OpenIddictConstants.Claims.Audience)))
                {
                    destinations.Add(OpenIddictConstants.Destinations.IdentityToken);
                }

                claim.SetDestinations(destinations);
            }

            var name = new Claim(OpenIddictConstants.Claims.GivenName, user.FullName ?? "[NA]");
            name.SetDestinations(OpenIddictConstants.Destinations.AccessToken);
            identity.AddClaim(name);

            return ticket;
        }

        private void AddUserClaims(User user, ClaimsIdentity identity)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (identity == null)
            {
                throw new ArgumentNullException(nameof(identity));
            }

            identity.AddClaim(ClaimTypesHelper.FirstName, user.FirstName);
            identity.AddClaim(ClaimTypesHelper.LastName, user.LastName);
            identity.AddClaim(ClaimTypesHelper.Email, user.Email);
            identity.AddClaim(ClaimTypesHelper.ID, user.Id.ToString());
            if (user.LastLoginDate.HasValue)
                identity.AddClaim(ClaimTypesHelper.LastLogin, user.LastLoginDate.Value.ToDateString("dd/MM/yyyy"));

            identity.AddClaim(ClaimTypesHelper.UserType, ((int)user.UserType).ToString());
        }
    }
}
