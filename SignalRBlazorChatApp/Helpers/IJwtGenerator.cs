namespace SignalRBlazorChatApp.Helpers
{
	public interface IJwtGenerator
	{
		string GetJwtToken(string userId, string userRole);
	}
}