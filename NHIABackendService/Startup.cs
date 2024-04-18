using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using NHIABackendService.Core.Collections;
using NHIABackendService.Utility;
using NHIABackendService.Services.Model;
using NHIABackendService.Core.DataAccess.EfCore.Context;

#nullable disable

namespace NHIABackendService
{
    public partial class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            HostingEnvironment = webHostEnvironment;
        }

        private IWebHostEnvironment HostingEnvironment { get; }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
            services.AddSwagger("NHIABackendService");

            ConfigureIdentity(services);
            AddIdentityProvider(services);
            ConfigureDIService(services);

            services.AddCors();
            services.AddAutoMapper(typeof(MappingConfig).Assembly);
            AddEntityFrameworkDbContext(services);


            services.AddControllers();
            services.AddHttpContextAccessor();

            services.AddAuthorization(options =>
            {
                var defaultAuthorizationPolicyBuilder =
                    new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme, "Bearer");
                defaultAuthorizationPolicyBuilder = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
                options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors(x =>
            {
                x.WithOrigins(Configuration["AllowedCorsOrigin"]
                        .Split(",", StringSplitOptions.RemoveEmptyEntries)
                        .Select(o => o.RemovePostFix("/"))
                        .ToArray())
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });

            app.UseCustomSwaggerApi();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers()
                    .RequireAuthorization();
            });

            System.Net.ServicePointManager.Expect100Continue = true;
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;
        }

        public void AddEntityFrameworkDbContext(IServiceCollection services)
        {
            var databaseConnectionString = Configuration.GetConnectionString("Default");

            services.AddDbContextPool<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(databaseConnectionString,
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).FullName).EnableRetryOnFailure());
            });
        }
    }
}
