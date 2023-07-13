using ChatApplicationModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SignalRBlazorGroupsMessages.API.Models;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;

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
        public virtual DbSet<PrivateGroupMembers> PrivateGroupsMembers { get; set; }
        public virtual DbSet<PrivateGroupMessages> PrivateGroupMessages { get; set; }
        public virtual DbSet<PrivateGroupRequests> PrivateGroupsRequests { get; set; }
        public virtual DbSet<PrivateUserMessages> PrivateUserMessages { get; set; }
        public virtual DbSet<PublicChatGroups> PublicChatGroups { get; set; }
        public virtual DbSet<PublicGroupMessages> PublicGroupMessages { get; set; }
        public virtual DbSet<UserFriends> UserFriends { get; set; }

        // stored procedures
        public virtual ObjectResult<PublicChatGroupsView> GetPublicChatGroups()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<PublicChatGroupsView>("sp_getPublicChatGroups");
        }
    }
}
