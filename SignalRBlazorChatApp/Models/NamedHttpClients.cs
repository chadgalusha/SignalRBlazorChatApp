namespace SignalRBlazorChatApp.Models
{
	public struct NamedHttpClients
	{
		public NamedHttpClients() { }

		public const string PublicGroupApi    = "publicGroupApi";
		public const string PublicMessageApi  = "publicMessageApi";
		public const string PrivateGroupApi	  = "privateGroupApi";
		public const string PrivateMessageApi = "privateMessageApi";
	}
}
