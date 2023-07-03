using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace SignalRBlazorGroupsMessages.API.Models
{
    public class PublicMessageDto
    {
        [Required]
        public string PublicMessageId { get; set; } = string.Empty;

        [Required]
        [DisplayName("User Id")]
        public string UserId { get; set; } = string.Empty;

        [DisplayName("User Name")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [DisplayName("Chat Group Id")]
        public int ChatGroupId { get; set; }

        [DisplayName("Chat Group Name")]
        public string ChatGroupName { get; set; } = string.Empty;

        [Required]
        [DisplayName("Message")]
        public string Text { get; set; } = string.Empty;

        [DisplayName("Message DateTime")]
        public DateTime MessageDateTime { get; set; }

        public string ReplyMessageId { get; set; } = string.Empty;

        public string PictureLink { get; set; } = string.Empty;
    }
}
