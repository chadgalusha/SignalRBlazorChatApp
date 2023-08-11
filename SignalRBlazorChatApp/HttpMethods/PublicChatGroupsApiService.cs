using Newtonsoft.Json;
using SignalRBlazorChatApp.Models;
using SignalRBlazorChatApp.Models.Dtos;

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
			ApiResponse<List<PublicChatGroupsDto>> apiResponse = new();

			_httpClient = HttpClientFactory.Create();
			_httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jsonWebToken);

			string uri = BaseUri();
			var dataRequest = await _httpClient.GetAsync(uri);

			if (dataRequest.IsSuccessStatusCode)
			{
				string jsonContent = await dataRequest.Content.ReadAsStringAsync();
				apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<PublicChatGroupsDto>>>(jsonContent)!;
			}

			if (apiResponse.Data == null)
			{
				apiResponse.Success = false;
				apiResponse.Message = "Error retriving data";
			}

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
