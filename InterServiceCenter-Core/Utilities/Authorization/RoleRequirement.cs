using Microsoft.AspNetCore.Authorization;

namespace InterServiceCenter_Core.Utilities.Authorization;

public class RoleRequirement : IAuthorizationRequirement
{
    public RoleRequirement(string role)
    {
        Role = role;
    }

    public string Role { get; set; }
}