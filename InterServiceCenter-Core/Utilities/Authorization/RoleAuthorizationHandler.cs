using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace InterServiceCenter_Core.Utilities.Authorization;

public class RoleAuthorizationHandler : AuthorizationHandler<RoleRequirement>
{
    private readonly JwtToken _token;
    private readonly GeneralUtilities _utilities;

    public RoleAuthorizationHandler(GeneralUtilities utilities, JwtToken token)
    {
        _utilities = utilities;
        _token = token;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        RoleRequirement requirement)
    {
        try
        {
            var claimsIdentity = context.User.Identity as ClaimsIdentity;
            var userRole = _token.GetLoggedRole(claimsIdentity);
            
            bool hasAllRoles = false;
            foreach (var role in requirement.Roles)
            {
                if (userRole == role)
                {
                    hasAllRoles = true;
                }
            }

            if (hasAllRoles)
                context.Succeed(requirement);
            else
                context.Fail(new AuthorizationFailureReason(this, "User does not have the required role."));
        }
        catch (Exception ex)
        {
            context.Fail(new AuthorizationFailureReason(this,
                "Authorization failed due to an internal error (Are you importing your session details?)."));
        }


        return Task.CompletedTask;
    }
}