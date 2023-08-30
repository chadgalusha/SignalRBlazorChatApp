using System.ComponentModel.DataAnnotations;

namespace SignalRBlazorGroupsMessages.API.Models.Dtos
{
    public class PrivateChatGroupsDto
    {
        [Key]
        public int ChatGroupId { get; set; }
        public string ChatGroupName { get; set; } = string.Empty;
        public DateTime GroupCreated { get; set; }
        public string GroupOwnerUserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
    }
}
