using Microsoft.AspNetCore.SignalR;
using SignalRBlazorChatApp.Models.Dtos;

namespace SignalRBlazorChatApp.Hubs
{
	public class PublicMessagesHub : Hub
	{
		public async Task SendMessage(PublicGroupMessageDto dto)
		{
			await Clients.All.SendAsync("NewMessage", dto);
		}

		public async Task EditMessage(PublicGroupMessageDto dto)
		{
			await Clients.All.SendAsync("EditMessage", dto);
		}

		public async Task DeleteMessage(int groupId, Guid messageId)
		{
			await Clients.All.SendAsync("DeleteMessage", groupId, messageId);
		}
	}
}
