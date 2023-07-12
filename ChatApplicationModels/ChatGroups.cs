using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ChatApplicationModels
{
    public class ChatGroups
    {
        [Key]
        public int ChatGroupId { get; set; }

        [Required]
        [DisplayName("Chat Group Name")]
        public string ChatGroupName { get; set; } = string.Empty;

        [DisplayName("Group Created")]
        public DateTime GroupCreated { get; set; }

        [DisplayName("Group Owner")]
        public Guid GroupOwnerUserId { get; set; }

        [DisplayName("Private Group")]
        public bool PrivateGroup { get;set; }
    }
}
