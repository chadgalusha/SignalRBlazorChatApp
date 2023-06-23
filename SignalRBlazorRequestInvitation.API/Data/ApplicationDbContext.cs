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

        public virtual DbSet<BlockedGroups> BlockedGroups { get; set; }
        public virtual DbSet<BlockedUsers> BlockedUsers { get; set; }
        public virtual DbSet<ChangeGroupOwnerRequests> ChangeGroupsOwnerRequests { get; set; }
        public virtual DbSet<ChatGroups> ChatGroups { get; set; }
        public virtual DbSet<FriendRequests> FriendRequests { get; set; }
        public virtual DbSet<PrivateGroupInvitations> PrivateGroupsInvitations { get; set; }
        public virtual DbSet<PrivateGroupMembers> PrivateGroupsMembers { get; set; }
        public virtual DbSet<PrivateGroupRequests> PrivateGroupsRequests { get; set; }
        public virtual DbSet<PrivateMessages> PrivateMessages { get; set; }
        public virtual DbSet<PublicMessages> PublicMessages { get; set; }
        public virtual DbSet<UserFriends> UserFriends { get; set; }
    }
}
