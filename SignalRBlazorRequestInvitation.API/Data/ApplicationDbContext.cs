using ChatApplicationModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SignalRBlazorRequestsInvitations.API.Models;

namespace SignalRBlazorRequestsInvitations.API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<BlockedPrivateChatGroups> BlockedPrivateChatGroups { get; set; }
        public virtual DbSet<BlockedUsers> BlockedUsers { get; set; }
        public virtual DbSet<ChangeGroupOwnerRequests> ChangeGroupOwnerRequests { get; set; }
        public virtual DbSet<FriendRequests> FriendRequests { get; set; }
        public virtual DbSet<PrivateChatGroups> PrivateChatGroups { get; set; }
        public virtual DbSet<PrivateGroupInvitations> PrivateGroupsInvitations { get; set; }
        public virtual DbSet<PrivateGroupMembers> PrivateGroupsMembers { get; set; }
        public virtual DbSet<PrivateGroupMessages> PrivateGroupMessages { get; set; }
        public virtual DbSet<PrivateGroupRequests> PrivateGroupsRequests { get; set; }
        public virtual DbSet<PrivateUserMessages> PrivateUserMessages { get; set; }
        public virtual DbSet<PublicChatGroups> PublicChatGroups { get; set; }
        public virtual DbSet<PublicGroupMessages> PublicGroupMessages { get; set; }
        public virtual DbSet<UserFriends> UserFriends { get; set; }
    }
}
