using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NHIABackendService.Core.Permissions
{
    public class AuthenticationHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuthenticationHandler> _logger;
        private readonly IConfiguration _configuration;

        public AuthenticationHandler(RequestDelegate next, ILogger<AuthenticationHandler> logger, IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        {

            var claim = new System.Security.Claims.Claim(ClaimTypes.Name, "");
            var claims = new List<System.Security.Claims.Claim>();
            claims.Add(claim);
            var claimsIdentity = new ClaimsIdentity(claims);
            context.User.AddIdentity(claimsIdentity);
            await _next(context);
        }
    }
}
