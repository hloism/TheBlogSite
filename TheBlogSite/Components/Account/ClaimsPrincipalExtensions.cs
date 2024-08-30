using System.Security.Claims;

namespace TheBlogSite.Components.Account
{
    public static class ClaimsPrincipalExtensions
    {
        //reflection nameIdentifier is the ID IMPORTANT METHOD 
        public static string? GetUserId(this ClaimsPrincipal principal)
        {
            return principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        }

    }
}
