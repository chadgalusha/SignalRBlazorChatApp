using Microsoft.AspNetCore.Identity;

namespace SignalRBlazorGroupsMessages.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsSearchable { get; set; }
        public bool CanReceiveGroupRequests { get; set; }
        public bool CanReceiveFriendRequests { get; set; }
        public bool CanReceiveMessages { get; set; }
    }
}
