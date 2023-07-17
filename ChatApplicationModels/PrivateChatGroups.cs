using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ChatApplicationModels
{
    public class PrivateChatGroups
    {
        [Key]
        public int ChatGroupId { get; set; }

        [Required]
        [DisplayName("Chat Group Name")]
        public string ChatGroupName { get; set; } = string.Empty;

        [DisplayName("Group Created")]
        public DateTime GroupCreated { get; set; }

        [DisplayName("Group Owner")]
        public string GroupOwnerUserId { get; set; } = string.Empty;
    }
}
