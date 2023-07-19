using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ChatApplicationModels
{
    public class BlockedPrivateChatGroups
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DisplayName("User Id")]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [DisplayName("Blocked Chat Group Id")]
        public int BlockedChatGroupId { get; set; }
    }
}