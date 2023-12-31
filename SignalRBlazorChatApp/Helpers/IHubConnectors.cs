﻿using Microsoft.AspNetCore.SignalR.Client;

namespace SignalRBlazorChatApp.Helpers
{
	public interface IHubConnectors
	{
		HubConnection? PrivateGroupMessagesConnect();
		HubConnection? PublicGroupMessagesConnect();
	}
}