using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using SignalRBlazorChatApp.Models;
using SignalRBlazorChatApp.Models.Dtos;
using System.Net.Http.Headers;
using System.Text;

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
			_httpClient = GetNewHttpClient(_httpClient, jsonWebToken);

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

		public async Task<ApiResponse<PublicGroupMessageDto>> PostNewMessage(CreatePublicGroupMessageDto createDto, string jsonWebToken)
		{
			_httpClient = GetNewHttpClient(_httpClient, jsonWebToken);

			string baseUri = BaseUri();

			var bodyMessage = new StringContent(
				JsonConvert.SerializeObject(createDto), Encoding.UTF8, "application/json");

			var postRequest = await _httpClient.PostAsync(baseUri, bodyMessage);

			string jsonContent = await postRequest.Content.ReadAsStringAsync();
			ApiResponse<PublicGroupMessageDto> apiResponse = JsonConvert
				.DeserializeObject<ApiResponse<PublicGroupMessageDto>>(jsonContent)!;

			return apiResponse;
		}

		public async Task<ApiResponse<PublicGroupMessageDto>> UpdateMessage(ModifyPublicGroupMessageDto modifyDto, string jsonWebToken)
		{
			_httpClient = GetNewHttpClient(_httpClient, jsonWebToken);

			string baseUri = BaseUri();

			var bodyMessage = new StringContent(
				JsonConvert.SerializeObject(modifyDto), Encoding.UTF8, "application/json");

			var updateRequest = await _httpClient.PutAsync(baseUri, bodyMessage);

			string jsonContent = await updateRequest.Content.ReadAsStringAsync();
			ApiResponse<PublicGroupMessageDto> apiResponse = JsonConvert
				.DeserializeObject<ApiResponse<PublicGroupMessageDto>>(jsonContent)!;

			return apiResponse;
		}

		public async Task<ApiResponse<PublicGroupMessageDto>> DeleteMessage(Guid messageId, string jsonWebToken)
		{
			_httpClient = GetNewHttpClient(_httpClient, jsonWebToken);

			var query = new Dictionary<string, string>()
			{
				["messageId"] = messageId.ToString()
			};

			string baseUri = BaseUri();
			var uriWithQuery = QueryHelpers.AddQueryString($"{baseUri}", query!);
			var deleteRequest = await _httpClient.DeleteAsync(uriWithQuery);

			if (!deleteRequest.IsSuccessStatusCode)
			{
				string jsonContent = await deleteRequest.Content.ReadAsStringAsync();
				ApiResponse<PublicGroupMessageDto> apiResponse = JsonConvert
					.DeserializeObject<ApiResponse<PublicGroupMessageDto>>(jsonContent)!;
				return apiResponse;
			}

			return new() { Success = true, Message = "ok" };
		}

		#region PRIVATE METHODS

		private string BaseUri()
		{
			return _configuration["ApiEndpointsConfig:PublicGroupMessagesUri"]!;
		}

		private HttpClient GetNewHttpClient(HttpClient httpClient, string jsonWebToken)
		{
			httpClient = HttpClientFactory.Create();
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jsonWebToken);
			return httpClient;
		}

		#endregion
	}
}
