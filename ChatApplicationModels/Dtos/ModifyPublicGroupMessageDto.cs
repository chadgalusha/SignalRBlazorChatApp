using System.ComponentModel.DataAnnotations;

namespace ChatApplicationModels.Dtos
{
    public class ModifyPublicGroupMessageDto
    {
        [Key]
        [Required]
        public Guid PublicMessageId { get; set; }

        public string Text { get; set; } = string.Empty;
        public Guid? ReplyMessageId { get; set; }

        public string PictureLink { get; set; } = string.Empty;
    }
}
