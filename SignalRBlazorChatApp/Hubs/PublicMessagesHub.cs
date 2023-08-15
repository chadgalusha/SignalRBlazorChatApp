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
	}
}
