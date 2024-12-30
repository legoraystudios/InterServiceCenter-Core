using Microsoft.AspNetCore.Authorization;

namespace InterServiceCenter_Core.Utilities.Authorization;

public class RoleRequirement : IAuthorizationRequirement
{
    public string[] Roles { get; }
    
    public RoleRequirement(params string[] roles)
    {
        Roles = roles;
    }
}