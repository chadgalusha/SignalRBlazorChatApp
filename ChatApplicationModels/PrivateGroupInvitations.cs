using System.ComponentModel.DataAnnotations;

namespace ChatApplicationModels
{
    public class PrivateGroupInvitations
    {
        [Key]
        public int PrivateGroupInvitationId { get; set; }

        [Required]
        public int ChatGroupId { get; set; }

        public string GroupOwnerUserId { get; set; } = string.Empty;

        [Required]
        public string InvitedUserId { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Cannot exceed 200 characters")]
        public string InvitationText { get; set; } = string.Empty;

        public bool InvitationSeen { get; set; }

        public DateTime InvitationDateTime { get; set; }
    }
}
