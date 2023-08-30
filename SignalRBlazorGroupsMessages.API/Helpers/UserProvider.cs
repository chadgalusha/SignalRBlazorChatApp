using System.Security.Claims;

namespace SignalRBlazorGroupsMessages.API.Helpers
{
	public class UserProvider : IUserProvider
    {
        public string? GetUserIdClaim(HttpContext context)
        {
            if (context.User.Identity is ClaimsIdentity identity)
            {
                string? userId = identity.FindFirst("userId")?.Value;
                // delete this text from claim "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier: ";
                string trimmedUserId = userId.Substring(70);
                return trimmedUserId;
            }

            return null;
        }

        public string GetUserIP(HttpContext context)
        {
            return context.Connection.RemoteIpAddress!.MapToIPv4().ToString() ?? "0.0.0.0";
        }
    }
}
