using Microsoft.AspNetCore.Identity;
using TaskMind.Domain.Identity;
namespace TaskMind.Web.Extensions;

public static class UserManagerExtensions
{
    public static Role? GetRole(this UserManager<User> userManager, System.Security.Claims.ClaimsPrincipal principal)
    {
        var userId = userManager.GetUserId(principal);
        var user = userManager.Users.FirstOrDefault(u => u.Id == userId);
        return user?.Role;
    }

    public static bool IsInRole(this UserManager<User> userManager, System.Security.Claims.ClaimsPrincipal principal, Role role)
    {
        var userRole = userManager.GetRole(principal);
        return userRole == role;
    }
}
