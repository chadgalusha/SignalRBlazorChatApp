using Microsoft.AspNetCore.Mvc;

namespace SignalRBlazorGroupsMessages.API.Helpers
{
    public interface IUserProvider
    {
        string? GetUserIdClaim(HttpContext context);
    }
}