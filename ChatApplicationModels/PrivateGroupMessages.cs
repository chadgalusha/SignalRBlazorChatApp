using System.ComponentModel.DataAnnotations;

namespace ChatApplicationModels
{
    public class PrivateGroupMessages
    {
        [Key]
        public Guid PrivateMessageId { get; set; }
        [Required]
        public string UserId { get; set; } = string.Empty;
        [Required]
        public int ChatGroupId { get; set; }
        [Required]
        public string Text { get; set; } = string.Empty;
        public DateTime MessageDateTime { get; set; }
        public Guid? ReplyMessageId { get; set; }
        public string PictureLink { get; set; } = string.Empty;
    }
}
