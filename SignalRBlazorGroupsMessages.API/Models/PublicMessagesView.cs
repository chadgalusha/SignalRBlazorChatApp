namespace SignalRBlazorGroupsMessages.API.Models
{
    public class PublicMessagesView
    {
        public string PublicMessageId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public int ChatGroupId { get; set; }
        public string ChatGroupName { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public DateTime MessageDateTime { get; set; }
        public string ReplyMessageId { get; set; } = string.Empty;
        public string PictureLink { get; set; } = string.Empty;
    }
}
