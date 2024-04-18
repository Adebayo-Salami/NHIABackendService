using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHIABackendService.Core.Utility
{
    public abstract class CoreConstants
    {
        public const string DateFormat = "dd MMMM, yyyy";
        public const string TimeFormat = "hh:mm tt";
        public const string SystemDateFormat = "dd/MM/yyyy";
        public const string Permission = nameof(Permission);
        public const string Role = "role";
        public const string UserIdKey = "oid";
        public static readonly string[] validExcels = { ".xls", ".xlsx" };

        public static readonly List<EmailTemplate> EmailTemplates = new List<EmailTemplate>
        {
            new EmailTemplate(MailUrl.PasswordReset, "Password Reset", "filestore/emailtemplates/passwordreset.htm")
        };

        public static class MailUrl
        {
            public const string PasswordReset = "filestore/emailtemplates/passwordreset.htm";
        }

        public class EmailTemplate
        {
            public EmailTemplate(string name, string subject, string template)
            {
                Subject = subject;
                TemplatePath = template;
                Name = name;
            }

            public string Name { get; set; }
            public string Subject { get; set; }
            public string TemplatePath { get; set; }
        }

        public static class PaginationConsts
        {
            public const int PageSize = 25;
            public const int PageIndex = 1;
        }

        public static class AllowedFileExtensions
        {
            public const string Signature = ".jpg,.png";
        }

        public static class CacheConstants
        {
            public static class Keys
            {
                public const string UserClaims = "userclaims";
                public const string RolePermissions = "rolepermissions";
                public const string UserRole = "role";
            }

            public static class CacheTime
            {
                public const int MonthInMinutes = 43800;
            }
        }
    }
}
