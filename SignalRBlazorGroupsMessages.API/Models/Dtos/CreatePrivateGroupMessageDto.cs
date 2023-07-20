using System.ComponentModel.DataAnnotations;

namespace SignalRBlazorGroupsMessages.API.Models.Dtos
{
    public class CreatePrivateGroupMessageDto
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int ChatGroupId { get; set; }

        [Required]
        public string Text { get; set; } = string.Empty;

        public Guid? ReplyMessageId { get; set; }

        public string PictureLink { get; set; } = string.Empty;
    }
}
