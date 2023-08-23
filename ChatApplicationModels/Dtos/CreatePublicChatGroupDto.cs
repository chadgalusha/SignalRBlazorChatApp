using System.ComponentModel.DataAnnotations;

namespace ChatApplicationModels.Dtos
{
    public class CreatePublicChatGroupDto
    {
        [Required]
        public string ChatGroupName { get; set; } = string.Empty;
        [Required]
        public string GroupOwnerUserId { get; set; } = string.Empty;
    }
}
