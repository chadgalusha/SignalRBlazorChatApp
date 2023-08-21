using System.ComponentModel.DataAnnotations;

namespace SignalRBlazorChatApp.Models.Dtos
{
	public class ModifyPublicChatGroupDto
	{
		[Key]
		[Required]
		public int ChatGroupId { get; set; }
		public string ChatGroupName { get; set; } = string.Empty;
	}
}
