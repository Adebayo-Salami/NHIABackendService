using System.ComponentModel;

namespace NHIABackendService.Core.Permissions
{
    public enum Permission
    {
        [Description("USER CREATE")] 
        USER_CREATE = 1,

        [Description("USER READ")] 
        USER_READ,

        [Description("USER UPDATE")] 
        USER_UPDATE,

        [Description("USER DELETE")]
        USER_DELETE,

        [Description("ROLE CREATE")] 
        ROLE_CREATE,

        [Description("ROLE READ")] 
        ROLE_READ,

        [Description("ROLE UPDATE")] 
        ROLE_UPDATE,

        [Description("ROLE DELETE")] 
        ROLE_DELETE,
    }
}
