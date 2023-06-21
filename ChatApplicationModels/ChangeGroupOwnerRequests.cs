using System.ComponentModel.DataAnnotations;

namespace ChatApplicationModels
{
    public class ChangeGroupOwnerRequests
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string CurrentOwnerUserId { get; set; } = string.Empty;
        
        [Required]
        public string NewOwnerUserId { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Cannot exceed 200 characters")]
        public string RequestText { get; set; } = string.Empty;

        public bool RequestSeen { get; set; }

        public DateTime ChangeOwnerDateTime { get; set; }
    }
}
