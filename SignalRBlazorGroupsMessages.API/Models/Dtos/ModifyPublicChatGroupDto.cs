using System.ComponentModel.DataAnnotations;

namespace SignalRBlazorGroupsMessages.API.Models.Dtos
{
    public class ModifyPublicChatGroupDto
    {
        [Key]
        [Required]
        public int ChatGroupId { get; set; }
        public string ChatGroupName { get; set; } = string.Empty;
    }
}
