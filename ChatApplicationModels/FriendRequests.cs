using System.ComponentModel.DataAnnotations;

namespace ChatApplicationModels
{
    public class FriendRequests
    {
        [Key]
        public int FriendRequestId { get; set; }

        [Required]
        public string RequestUserId { get; set; } = string.Empty;

        [Required]
        public string RecipientUserId { get; set; } = string.Empty;

        public bool RequestSeen { get; set; }

        public DateTime FriendRequestDateTime { get; set; }
    }
}
