using ChatApplicationModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SignalRBlazorGroupsMessages.API.Models;

namespace SignalRBlazorGroupsMessages.API.Data
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
        public virtual DbSet<PrivateGroupMembers> PrivateGroupMembers { get; set; }
        public virtual DbSet<PrivateGroupMessages> PrivateGroupMessages { get; set; }
        public virtual DbSet<PrivateGroupRequests> PrivateGroupRequests { get; set; }
        public virtual DbSet<PrivateUserMessages> PrivateUserMessages { get; set; }
        public virtual DbSet<PublicChatGroups> PublicChatGroups { get; set; }
        public virtual DbSet<PublicGroupMessages> PublicGroupMessages { get; set; }
        public virtual DbSet<UserFriends> UserFriends { get; set; }
    }
}
