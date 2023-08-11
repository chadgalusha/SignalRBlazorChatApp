using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace SignalRBlazorChatApp.Helpers
{
    public static class UserProcessor
    {
        public static (string, string) GetUserIdAndRole(AuthenticationState authState)
        {
            var user = authState.User;
            var claims = user.Claims;

            string userId = claims.First(c => c.Type == ClaimTypes.NameIdentifier).ToString();

            string userRole = claims.Where(c => c.Type == ClaimTypes.Role)
                .Select(_ => _.Value).First();

            if (userId.IsNullOrEmpty())
            {
                userId = "";
            }
            if (userRole.IsNullOrEmpty())
            {
                userRole = "";
            }

            return (userId, userRole)!;
        }
    }
}
