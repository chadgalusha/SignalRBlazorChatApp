using System.Net.Http.Headers;

namespace SignalRBlazorChatApp.HttpMethods
{
	public class ChatHttpMethods : IChatHttpMethods
	{
		private readonly IHttpClientFactory _httpClientFactory;

		public ChatHttpMethods(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory ?? throw new Exception(nameof(httpClientFactory));
		}

		public async Task<HttpResponseMessage> GetAsync(string jsonWebToken, string httpClient)
		{
			var client = GetClient(jsonWebToken, httpClient);
			return await client.GetAsync("");
		}

		public async Task<HttpResponseMessage> GetAsync(string jsonWebToken, string httpClient, string path)
		{
			var client = GetClient(jsonWebToken, httpClient);
			return await client.GetAsync(path);
		}

		public async Task<HttpResponseMessage> PostAsync(string jsonWebToken, string httpClient, StringContent bodyMessage)
		{
			var client = GetClient(jsonWebToken, httpClient);
			return await client.PostAsync("", bodyMessage);
		}

		public async Task<HttpResponseMessage> PutAsync(string jsonWebToken, string httpClient, StringContent bodyMessage)
		{
			var client = GetClient(jsonWebToken, httpClient);
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jsonWebToken);
			return await client.PutAsync("", bodyMessage);
		}

		public async Task<HttpResponseMessage> DeleteAsync(string jsonWebToken, string httpClient, string queryString)
		{
			var client = GetClient(jsonWebToken, httpClient);
			return await client.DeleteAsync(queryString);
		}

		#region PRIVATE METHODS

		private HttpClient GetClient(string jsonWebToken, string httpClient)
		{
			var client = _httpClientFactory.CreateClient(httpClient);
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jsonWebToken);
			return client;
		}

		#endregion
	}
}
