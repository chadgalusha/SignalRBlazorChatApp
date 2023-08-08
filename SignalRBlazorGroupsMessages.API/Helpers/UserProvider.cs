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

        public string GetUserIP(HttpContext context)
        {
            return context.Connection.RemoteIpAddress!.MapToIPv4().ToString() ?? "0.0.0.0";
        }
    }
}
