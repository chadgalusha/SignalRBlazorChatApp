﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ChatApplicationModels
{
    public class PublicChatGroups
    {
        [Key]
        public int ChatGroupId { get; set; }

        [Required]
        [DisplayName("Chat Group Name")]
        public string ChatGroupName { get; set; } = string.Empty;

        [DisplayName("Group Created")]
        public DateTime GroupCreated { get; set; }

        [DisplayName("Group Owner")]
        public string GroupOwnerUserId { get; set; } = string.Empty;
    }
}
