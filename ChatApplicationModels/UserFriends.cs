using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ChatApplicationModels
{
    public class UserFriends
    {
        [Key]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string FriendUserId { get; set; } = string.Empty;

        [DisplayName("Friends Since")]
        public DateTime FriendSinceDateTime { get; set; }
    }
}
