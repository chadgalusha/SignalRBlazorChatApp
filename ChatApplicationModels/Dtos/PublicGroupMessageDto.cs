using System.ComponentModel.DataAnnotations;

namespace ChatApplicationModels.Dtos
{
	public class PublicGroupMessageDto
    {
        [Key]
        [Required]
        public Guid PublicMessageId { get; set; }
        [Required]
        public string UserId { get; set; } = string.Empty;
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
