using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Primitives;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NHIABackendService.Core.Configuration;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;
using OpenIddict.Validation;

namespace NHIABackendService
{
    public partial class Startup
    {
        public async Task InitializeIdentityDbAsync(IServiceProvider services, CancellationToken cancellationToken)
        {
            using var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var manager =
                scope.ServiceProvider.GetRequiredService<OpenIddictApplicationManager<OpenIddictApplication>>();

            if (await manager.FindByClientIdAsync("NHIA", cancellationToken) == null)
            {
                var descriptor = new OpenIddictApplicationDescriptor
                {
                    ClientId = "NHIA",
                    ClientSecret = "571e876d-04b5-40ff-c431-7ac541fbd1c9",
                    DisplayName = "NHIA Api client",
                    Type = OpenIddictConstants.ClientTypes.Hybrid,
                    Permissions =
                    {
                        OpenIddictConstants.Permissions.Endpoints.Token,
                        OpenIddictConstants.Permissions.GrantTypes.Password,
                        OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                        OpenIddictConstants.Permissions.Scopes.Email,
                        OpenIddictConstants.Permissions.Scopes.Profile,
                        OpenIddictConstants.Permissions.Scopes.Roles
                    }
                };

                await manager.CreateAsync(descriptor, cancellationToken);
            }
        }

        public void AddIdentityProvider(IServiceCollection services)
        {
            var authSettings = new OpenIddictServerConfig();

            Configuration.Bind(nameof(OpenIddictServerConfig), authSettings);
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(authSettings.SecretKey));

            services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = OpenIdConnectConstants.Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;

                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequiredLength = 6;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                options.Lockout.MaxFailedAccessAttempts = 3;
            });
        }

        public void ConfigureIdentity(IServiceCollection services)
        {
            //services.AddIdentity<User, Role>(options =>
            //{
            //    options.Password.RequireDigit = false;
            //    options.Password.RequireLowercase = false;
            //    options.Password.RequireUppercase = false;
            //    options.Password.RequireNonAlphanumeric = false;
            //    options.Password.RequiredLength = 6;

            //    options.Lockout.AllowedForNewUsers = true;
            //    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
            //    options.Lockout.MaxFailedAccessAttempts = 3;
            //})
            //    .AddEntityFrameworkStores<ApplicationDbContext>()
            //    .AddDefaultTokenProviders();

            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromHours(24);
            });
        }
    }
}
