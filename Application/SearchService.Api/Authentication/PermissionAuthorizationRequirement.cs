using Microsoft.AspNetCore.Authorization;

namespace SearchService.Api.Authentication;

public class PermissionAuthorizationRequirement(params string[] allowedPermissions)
    : AuthorizationHandler<PermissionAuthorizationRequirement>, IAuthorizationRequirement
{
    public string[] AllowedPermissions { get; } = allowedPermissions;

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionAuthorizationRequirement requirement)
    {
        foreach (var claim in context.User.Claims)
        {
            Console.WriteLine(claim.ToString());
        }
        //Console.WriteLine(context.User.ToString());

        foreach (var permission in requirement.AllowedPermissions)
        {
            Console.WriteLine(permission);
            bool found = context.User.FindFirst(c =>
                c.Type == CustomClaimTypes.Permission &&
                c.Value == permission) is not null;

            if (found)
            {
                context.Succeed(requirement);
                break;
            }
        }
        return Task.CompletedTask;
    }
}
