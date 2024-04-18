namespace NHIABackendService.Core.Configuration
{
    public class OpenIddictServerConfig
    {
        public string SecretKey { get; set; }
        public string Authority { get; set; }
        public bool RequireHttps { get; set; }
    }

    public class ActiveDirectoryConfig
    {
        public const string Authenticate = "/api/Service/AuthenticateUser";
        public bool AllowADLogin { get; set; }
        public string AppId { get; set; }
        public string BaseUrl { get; set; }
        public bool AuthenticateApi { get; set; }
        public string ApiUser { get; set; }
        public string ApiPassword { get; set; }
    }

    public class AzureAdOptions
    {
        public string Instance { get; set; }
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string CallbackPath { get; set; }
        public string BaseUrl { get; set; }
        public string ClientSecret { get; set; }
        public string GraphScopes { get; set; }
    }
}
