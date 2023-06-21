using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ChatApplicationModels
{
    public class PrivateMessages
    {
        [Key]
        public int PrivateMessageId { get; set; }

        [Required]
        [DisplayName("From User")]
        public string FromUserId { get; set; } = string.Empty;

        [Required]
        [DisplayName("To User")]
        public string ToUserId { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Cannot exceed 200 characters")]
        [DisplayName("Message Text")]
        public string MessageText { get; set; } = string.Empty;

        public bool MessageSeen { get; set; }

        [DisplayName("Message DateTime")]
        public DateTime PrivateMessageDateTime { get; set; }
    }
}
