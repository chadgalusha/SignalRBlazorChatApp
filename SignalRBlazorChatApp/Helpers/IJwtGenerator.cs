using Microsoft.AspNetCore.Components.Authorization;

namespace SignalRBlazorChatApp.Helpers
{
	public interface IJwtGenerator
	{
		string GetJwtToken(AuthenticationState authState);
	}
}