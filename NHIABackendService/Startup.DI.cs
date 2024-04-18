﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using NHIABackendService.Core.AspnetCore;
using NHIABackendService.Core.Caching;
using NHIABackendService.Core.DataAccess.EfCore;
using NHIABackendService.Core.DataAccess.EfCore.Context;
using NHIABackendService.Core.DataAccess.EfCore.UnitOfWork;
using NHIABackendService.Core.FileStorage;
using NHIABackendService.Core.Permissions;
using NHIABackendService.Core.Utility;
using NHIABackendService.Services.Interface;
using NHIABackendService.Services.Services;
using System.Data;
using System.Data.SqlClient;

namespace NHIABackendService
{
    public partial class Startup
    {
        public void ConfigureDIService(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<DbContext, ApplicationDbContext>();
            services.AddScoped<IUnitOfWork, EfCoreUnitOfWork>();
            services.AddScoped(typeof(IDbContextProvider<>), typeof(UnitOfWorkDbContextProvider<>));
            services.RegisterGenericRepos(typeof(ApplicationDbContext));
            services.AddScoped<IHttpUserService, HttpUserService>();
            services.AddScoped<IHttpUserService, HttpUserService>();
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
            services.AddSingleton<ICacheManager, MemoryCacheManager>();
            services.AddScoped<IBaseRequestService, BaseRequestService>();
            services.AddScoped<IFileStorageService, FileStorageService>();
            services.AddTransient<IFileService, FileService>();

            services.AddScoped<IDbConnection>(db =>
            {
                var connectionString = Configuration.GetConnectionString("Default");
                var connection = new SqlConnection(connectionString);
                return connection;
            });
        }
    }
}
