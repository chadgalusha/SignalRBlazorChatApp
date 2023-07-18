using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace SignalRBlazorGroupsMessages.API.Models.Dtos
{
    public class PublicGroupMessageDto
    {
        [Required]
        public Guid PublicMessageId { get; set; }
        [Required]
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        [Required]
        public int ChatGroupId { get; set; }
        public string ChatGroupName { get; set; } = string.Empty;
        [Required]
        public string Text { get; set; } = string.Empty;
        public DateTime MessageDateTime { get; set; }
        public Guid? ReplyMessageId { get; set; }
        public string PictureLink { get; set; } = string.Empty;
    }
}
