using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ChatApplicationModels
{
    public class PrivateGroupMessages
    {
        [Key]
        public Guid PrivateMessageId { get; set; }

        [Required]
        [DisplayName("User")]
        public Guid UserId { get; set; }

        [Required]
        [DisplayName("Chat Group")]
        public int ChatGroupId { get; set; }

        [Required]
        [DisplayName("Message")]
        public string Text { get; set; } = string.Empty;

        [DisplayName("Message DateTime")]
        public DateTime MessageDateTime { get; set; }

        public Guid? ReplyMessageId { get; set; }

        public string PictureLink { get; set; } = string.Empty;
    }
}
