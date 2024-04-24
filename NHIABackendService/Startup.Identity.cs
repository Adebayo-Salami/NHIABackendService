using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NHIABackendService.Core.Configuration;
using NHIABackendService.Core.DataAccess.EfCore.Context;
using NHIABackendService.Entities;
using OpenIddict.Abstractions;

namespace NHIABackendService
{
    public partial class Startup
    {
        public void AddIdentityProvider(IServiceCollection services)
        {
            var authSettings = new OpenIddictServerConfig();

            Configuration.Bind(nameof(OpenIddictServerConfig), authSettings);
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(authSettings.SecretKey));

            services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserNameClaimType = OpenIddictConstants.Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = OpenIddictConstants.Claims.Role;

                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequiredLength = 6;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                options.Lockout.MaxFailedAccessAttempts = 3;
            });

            var x509Certificate = new X509Certificate2(Path.Combine(
                    HostingEnvironment.ContentRootPath, "NHIAAuth.pfx")
                , "NHIABackendService", X509KeyStorageFlags.UserKeySet);

            services.AddOpenIddict()
                .AddCore(options =>
                {
                    options.UseEntityFrameworkCore()
                        .UseDbContext<ApplicationDbContext>();
                })
                .AddServer(options =>
                {
                    options.RegisterScopes(OpenIddictConstants.Scopes.Email,
                        OpenIddictConstants.Scopes.Profile,
                        OpenIddictConstants.Scopes.Address,
                        OpenIddictConstants.Scopes.Phone,
                        OpenIddictConstants.Scopes.Roles,
                        OpenIddictConstants.Scopes.OfflineAccess,
                        OpenIddictConstants.Scopes.OpenId
                    );

                    options.SetTokenEndpointUris("/api/connect/token")
                        //.SetLogoutEndpointUris("connect/logout")
                        .AllowRefreshTokenFlow()
                        .AcceptAnonymousClients()
                        .AllowPasswordFlow()
#if DEBUG
                        .SetAccessTokenLifetime(TimeSpan.FromMinutes(720)) // 12 hours
                        .SetIdentityTokenLifetime(TimeSpan.FromMinutes(720)) // 12 hours
                        .SetRefreshTokenLifetime(TimeSpan.FromMinutes(720)) // 12 hours
#else
                        .SetAccessTokenLifetime(TimeSpan.FromMinutes(60))
                        .SetIdentityTokenLifetime(TimeSpan.FromMinutes(60))
                        .SetRefreshTokenLifetime(TimeSpan.FromMinutes(120))
#endif
                        .AddSigningCertificate(x509Certificate)
                        .AddEncryptionCertificate(x509Certificate);

                    options.UseAspNetCore()
                       .EnableStatusCodePagesIntegration()
                       .EnableAuthorizationEndpointPassthrough()
                       .EnableLogoutEndpointPassthrough()
                       .EnableTokenEndpointPassthrough();

                    options.DisableAccessTokenEncryption();

                    options.UseDataProtection()
                       .PreferDefaultAccessTokenFormat()
                       .PreferDefaultAuthorizationCodeFormat()
                       .PreferDefaultDeviceCodeFormat()
                       .PreferDefaultRefreshTokenFormat()
                       .PreferDefaultUserCodeFormat();
                })
                .AddValidation(options =>
                {
                    // Import the configuration from the local OpenIddict server instance.
                    options.UseLocalServer();

                    // Register the ASP.NET Core host.
                    options.UseAspNetCore();
                });

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = OpenIddictConstants.Schemes.Bearer;
                x.DefaultChallengeScheme = OpenIddictConstants.Schemes.Bearer;
                x.DefaultSignInScheme = OpenIddictConstants.Schemes.Bearer;
            }).AddJwtBearer("Bearer", options =>
            {
                options.Authority = options.Authority = authSettings.Authority;
                options.RequireHttpsMetadata = authSettings.RequireHttps;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = OpenIddictConstants.Claims.Name,
                    RoleClaimType = OpenIddictConstants.Claims.Role,
                    IssuerSigningKey = signingKey,
                    ValidateAudience = false,
                    ValidateIssuer = false
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            context.Response.Headers.Append("Token-Expired", "true");
                        return Task.CompletedTask;
                    }
                };
            });
        }

        public void ConfigureIdentity(IServiceCollection services)
        {
            services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;

                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                options.Lockout.MaxFailedAccessAttempts = 3;
            }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromHours(24);
            });
        }
    }
}
