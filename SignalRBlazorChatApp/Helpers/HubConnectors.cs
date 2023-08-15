using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace SignalRBlazorChatApp.Helpers
{
	public class HubConnectors : IHubConnectors
	{
		private readonly NavigationManager _navigation;

		public HubConnectors(NavigationManager navigation)
		{
			_navigation = navigation ?? throw new Exception(nameof(navigation));
		}

		public HubConnection? PublicGroupMessagesConnect()
		{
			return new HubConnectionBuilder()
				.WithUrl(_navigation.ToAbsoluteUri("/publicmessageshub"))
				.Build();
		}
	}
}
