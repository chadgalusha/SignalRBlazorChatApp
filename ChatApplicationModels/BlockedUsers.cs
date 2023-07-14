using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ChatApplicationModels
{
    public class BlockedUsers
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DisplayName("User Id")]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [DisplayName("Blocked User Id")]
        public string BlockedUserId { get; set; } = string.Empty;
    }
}
