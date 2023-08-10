namespace SignalRBlazorChatApp.HttpMethods
{
	public class PublicChatGroupsApiService
	{
		private HttpClient _httpClient;
		private readonly IConfiguration _configuration;

		public PublicChatGroupsApiService(HttpClient httpClient, IConfiguration configuration)
		{
			_httpClient = httpClient ?? throw new Exception(nameof(httpClient));
			_configuration = configuration ?? throw new Exception(nameof(configuration));
		}

		public async Task GetPublicChatGroupsAsync(string jsonWebToken)
		{
			_httpClient = HttpClientFactory.Create();
			_httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jsonWebToken);
		}

		#region PRIVATE METHODS

		private string BaseUri()
		{
			return _configuration["ApiEndpointsConfig:PublicChatGroupsUri"]!;
		}

		#endregion
	}
}
