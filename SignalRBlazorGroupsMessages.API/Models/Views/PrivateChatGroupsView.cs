using System.ComponentModel.DataAnnotations;

namespace SignalRBlazorGroupsMessages.API.Models.Views
{
    public class PrivateChatGroupsView
    {
        [Key]
        public int ChatGroupId { get; set; }
        public string ChatGroupName { get; set; } = string.Empty;
        public DateTime GroupCreated { get; set; }
        public Guid GroupOwnerUserId { get; set; }
        public string UserName { get; set; } = string.Empty;
    }
}
