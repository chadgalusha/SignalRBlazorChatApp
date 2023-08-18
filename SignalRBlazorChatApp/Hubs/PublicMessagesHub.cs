using Microsoft.AspNetCore.SignalR;
using SignalRBlazorChatApp.Models.Dtos;

namespace SignalRBlazorChatApp.Hubs
{
	public class PublicMessagesHub : Hub
	{
		public async Task AddToGroup(string groupId)
		{
			await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
		}

		public async Task RemoveFromGroup(string groupName)
		{
			await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
		}

		public async Task SendGroupMessageAdd(string groupId, PublicGroupMessageDto dto)
		{
			await Clients.Group(groupId).SendAsync("ReceiveAdd", dto);
		}

		public async Task SendGroupMessageEdit(string groupId, PublicGroupMessageDto dto)
		{
			await Clients.Group(groupId).SendAsync("ReceiveEdit", dto);
		}

		public async Task SendGroupMessageDelete(string groupId, Guid messageId)
		{
			await Clients.Group(groupId).SendAsync("ReceiveDelete", messageId);
		}
	}
}
