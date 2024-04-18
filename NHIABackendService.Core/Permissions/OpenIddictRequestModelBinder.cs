using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHIABackendService.Core.Permissions
{
    public class OpenIddictRequestModelBinder : IModelBinderProvider, IModelBinder
    {
        private readonly ILogger<OpenIddictRequestModelBinder> _logger;

        public OpenIddictRequestModelBinder(ILogger<OpenIddictRequestModelBinder> logger)
        {
            _logger = logger;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            if (bindingContext.ModelType != typeof(OpenIddictRequest))
            {
                return Task.CompletedTask;
            }

            // Your custom binding logic here
            // For example, fetch data based on the request parameters
            // and create an instance of OpenIddictRequest
            // ...

            // Create an instance of OpenIddictRequest (replace with actual logic)
            var request = new OpenIddictRequest()
            {
                ClientId = bindingContext.ValueProvider.GetValue("client_id").FirstValue,
                GrantType = bindingContext.ValueProvider.GetValue("grant_type").FirstValue,
                Scope = bindingContext.ValueProvider.GetValue("scope").FirstValue,
                Password = bindingContext.ValueProvider.GetValue("password").FirstValue,
                Username = bindingContext.ValueProvider.GetValue("username").FirstValue,
                RefreshToken = bindingContext.ValueProvider.GetValue("refresh_token").FirstValue,
                IdToken = bindingContext.ValueProvider.GetValue("id_token").FirstValue,
            };

            // Set the model value
            bindingContext.Result = ModelBindingResult.Success(request);
            return Task.CompletedTask;
        }

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(OpenIddictRequest))
            {
                var loggerFactory = context.Services.GetRequiredService<ILogger<OpenIddictRequestModelBinder>>();
                return new OpenIddictRequestModelBinder(loggerFactory);
            }

            return null;
        }
    }

    public class OpenIddictRequestModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(OpenIddictRequest))
            {
                var loggerFactory = context.Services.GetRequiredService<ILogger<OpenIddictRequestModelBinder>>();
                return new OpenIddictRequestModelBinder(loggerFactory);
            }

            return null;
        }
    }
}
