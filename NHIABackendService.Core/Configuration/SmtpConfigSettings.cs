namespace NHIABackendService.Core.Configuration
{
    public class SmtpConfig
    {
        public bool UseSSl { get; set; }
        public int Port { get; set; }
        public string Server { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public bool UseDefaultCredentials { get; set; }
    }
}
