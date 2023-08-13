using Newtonsoft.Json;
using SignalRBlazorChatApp.Models;
using SignalRBlazorChatApp.Models.Dtos;
using System.Net.Http.Headers;

namespace SignalRBlazorChatApp.HttpMethods
{
	public class PublicChatGroupsApiService : IPublicChatGroupsApiService
	{
		private HttpClient _httpClient;
		private readonly IConfiguration _configuration;

		public PublicChatGroupsApiService(HttpClient httpClient, IConfiguration configuration)
		{
			_httpClient = httpClient ?? throw new Exception(nameof(httpClient));
			_configuration = configuration ?? throw new Exception(nameof(configuration));
		}

		public async Task<ApiResponse<List<PublicChatGroupsDto>>> GetPublicChatGroupsAsync(string jsonWebToken)
		{
			_httpClient = HttpClientFactory.Create();
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jsonWebToken);

			string uri = BaseUri();
			var dataRequest = await _httpClient.GetAsync(uri);

			string jsonContent = await dataRequest.Content.ReadAsStringAsync();
			ApiResponse<List<PublicChatGroupsDto>> apiResponse = JsonConvert
				.DeserializeObject<ApiResponse<List<PublicChatGroupsDto>>>(jsonContent)!;

			return apiResponse;
		}

		#region PRIVATE METHODS

		private string BaseUri()
		{
			return _configuration["ApiEndpointsConfig:PublicChatGroupsUri"]!;
		}

		#endregion
	}
}
