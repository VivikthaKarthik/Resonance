using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ResoClassAPI.Authentication
{
    public class HasPermissionHandler : AuthorizationHandler<HasPermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasPermissionRequirement requirement)
        {
            if (!context.User.HasClaim(c => c.Type == ClaimTypes.Role))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            string role = context.User.FindFirst(ClaimTypes.Role).Value.ToString();

            // Check if any of the user's roles have the required permission
            if (!string.IsNullOrEmpty(role) && CheckRolePermission(role, requirement.Permission))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

            return Task.CompletedTask;
        }

        private bool CheckRolePermission(string role, string permission)
        {
            return role == permission;
        }
    }
}
