using Core.Contracts.Enums;
using Microsoft.AspNetCore.Authorization;

namespace Core.Services.Auth;

public class AuthorizeRoles : AuthorizeAttribute
{
    private static readonly Dictionary<UserRole, string> RolesMap = new()
    {
        { UserRole.User, nameof(UserRole.User) },
        { UserRole.Moderator, nameof(UserRole.Moderator) },
        { UserRole.Support, nameof(UserRole.Support) },
        { UserRole.Admin, nameof(UserRole.Admin) },
    };

    public AuthorizeRoles(params UserRole[] roles)
    {
        Roles = string.Join(",", roles.Select(role => RolesMap[role]));
    }
}