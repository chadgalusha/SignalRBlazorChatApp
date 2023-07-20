using System.ComponentModel.DataAnnotations;

namespace SignalRBlazorGroupsMessages.API.Models.Dtos
{
    public class ModityPrivateGroupMessageDto
    {
        [Key]
        [Required]
        public Guid PrivateMessageId { get; set; }

        public string Text { get; set; } = string.Empty;
        public Guid? ReplyMessageId { get; set; }

        public string PictureLink { get; set; } = string.Empty;
    }
}
