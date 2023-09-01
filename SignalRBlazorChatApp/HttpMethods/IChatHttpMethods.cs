namespace SignalRBlazorChatApp.HttpMethods
{
	public interface IChatHttpMethods
	{
		Task<HttpResponseMessage> DeleteAsync(string jsonWebToken, string httpClient, string queryString);
		Task<HttpResponseMessage> GetAsync(string jsonWebToken, string httpClient);
		Task<HttpResponseMessage> GetAsync(string jsonWebToken, string httpClient, string path);
		Task<HttpResponseMessage> PostAsync(string jsonWebToken, string httpClient, StringContent bodyMessage);
		Task<HttpResponseMessage> PostAsync(string jsonWebToken, string httpClient, StringContent bodyMessage, string path);
		Task<HttpResponseMessage> PutAsync(string jsonWebToken, string httpClient, StringContent bodyMessage);
	}
}