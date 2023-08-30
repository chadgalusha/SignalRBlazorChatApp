using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ChatApplicationModels
{
    public class PublicChatGroups
    {
        [Key]
        public int ChatGroupId { get; set; }
        [Required]
        public string ChatGroupName { get; set; } = string.Empty;
        public DateTime GroupCreated { get; set; }
        public string GroupOwnerUserId { get; set; } = string.Empty;
    }
}
