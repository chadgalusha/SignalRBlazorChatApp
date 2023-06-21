using System.ComponentModel.DataAnnotations;

namespace ChatApplicationModels
{
    public class PrivateGroupMembers
    {
        [Key]
        public int PrivateGroupMemberId { get; set; }

        [Required]
        public int PrivateChatGroupId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
    }
}
