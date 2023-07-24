using System.Security.Claims;

namespace SignalRBlazorGroupsMessages.API.Helpers
{
    public class UserProvider : IUserProvider
    {
        public string? GetUserIdClaim(HttpContext context)
        {
            if (context.User.Identity is ClaimsIdentity identity)
            {
                return identity.FindFirst("userId")?.Value;
            }

            return null;
        }
    }
}
