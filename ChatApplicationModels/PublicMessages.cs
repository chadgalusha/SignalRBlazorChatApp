using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ChatApplicationModels
{
    public class PublicMessages
    {
        [Key]
        public string PublicMessageId { get; set; } = string.Empty;

        [Required]
        [DisplayName("User")]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [DisplayName("Chat Group")]
        public int ChatGroupId  { get; set; }

        [Required]
        [DisplayName("Message")]
        public string Text { get; set; } = string.Empty;

        [DisplayName("Message DateTime")]
        public DateTime MessageDateTime { get; set; }

        public string ReplyMessageId { get; set; } = string.Empty;

        public string PictureLink { get; set; } = string.Empty;
    }
}
