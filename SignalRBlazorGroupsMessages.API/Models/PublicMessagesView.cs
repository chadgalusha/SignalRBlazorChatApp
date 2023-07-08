using System.ComponentModel.DataAnnotations;

namespace SignalRBlazorGroupsMessages.API.Models
{
    public class PublicMessagesView
    {
        [Key]
        public Guid PublicMessageId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int ChatGroupId { get; set; }
        public string ChatGroupName { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public DateTime MessageDateTime { get; set; }
        public Guid? ReplyMessageId { get; set; }
        public string PictureLink { get; set; } = string.Empty;
    }
}
