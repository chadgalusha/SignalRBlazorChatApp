using System.ComponentModel.DataAnnotations;

namespace ChatApplicationModels.Dtos
{
    public class ModifyPrivateChatGroupDto
    {
        [Key]
        [Required]
        public int ChatGroupId { get; set; }
        public string ChatGroupName { get; set; } = string.Empty;
    }
}
