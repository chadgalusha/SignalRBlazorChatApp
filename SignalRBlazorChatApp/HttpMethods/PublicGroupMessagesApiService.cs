using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using SignalRBlazorChatApp.Models;
using SignalRBlazorChatApp.Models.Dtos;
using System.Net.Http.Headers;

namespace SignalRBlazorChatApp.HttpMethods
{
	public class PublicGroupMessagesApiService : IPublicGroupMessagesApiService
	{
		private HttpClient _httpClient;
		private readonly IConfiguration _configuration;

		public PublicGroupMessagesApiService(HttpClient httpClient, IConfiguration configuration)
		{
			_httpClient = httpClient ?? throw new Exception(nameof(httpClient));
			_configuration = configuration ?? throw new Exception(nameof(configuration));
		}

		public async Task<ApiResponse<List<PublicGroupMessageDto>>> GetMessagesByGroupId(
			int groupId, int numberItemsToSkip, string jsonWebToken)
		{
			_httpClient = HttpClientFactory.Create();
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jsonWebToken);

			var query = new Dictionary<string, string>()
			{
				["groupId"] = groupId.ToString(),
				["numberItemsToSkip"] = numberItemsToSkip.ToString()
			};

			string baseUri = BaseUri();
			var uriWithQuery = QueryHelpers.AddQueryString($"{baseUri}bygroupid", query!);
			var dataRequest = await _httpClient.GetAsync(uriWithQuery);

			string jsonContent = await dataRequest.Content.ReadAsStringAsync();
			ApiResponse<List<PublicGroupMessageDto>> apiResponse = JsonConvert
				.DeserializeObject<ApiResponse<List<PublicGroupMessageDto>>>(jsonContent)!;

			return apiResponse;
		}

		#region PRIVATE METHODS

		private string BaseUri()
		{
			return _configuration["ApiEndpointsConfig:PublicGroupMessagesUri"]!;
		}

		#endregion
	}
}
