using Microsoft.AspNetCore.Authorization;

namespace ResoClassAPI.Authentication
{
    public class HasPermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }

        public HasPermissionRequirement(string permission)
        {
            Permission = permission;
        }
    }
}
