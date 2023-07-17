using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace SignalRBlazorGroupsMessages.API.Models.Dtos
{
    public class PublicGroupMessageDto
    {
        [Required]
        public Guid PublicMessageId { get; set; }

        [Required]
        [DisplayName("User Id")]
        public Guid UserId { get; set; }

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

        public Guid? ReplyMessageId { get; set; }

        public string PictureLink { get; set; } = string.Empty;
    }
}
