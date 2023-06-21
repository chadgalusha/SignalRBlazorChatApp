using System.ComponentModel.DataAnnotations;

namespace ChatApplicationModels
{
    public class PrivateGroupRequests
    {
        [Key]
        public int PrivateGroupRequestId { get; set; }

        [Required]
        public int ChatGroupId { get; set; }

        [Required]
        public string RequestUserId { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Cannot exceed 200 characters")]
        public string RequestText { get; set; } = string.Empty;

        public DateTime GroupRequestDateTime { get; set; }
    }
}
