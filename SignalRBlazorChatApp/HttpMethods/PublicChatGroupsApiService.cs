using ChatApplicationModels.Dtos;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using SignalRBlazorChatApp.Models;
using System.Net.Http.Headers;
using System.Text;

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
			_httpClient = GetNewHttpClient(_httpClient, jsonWebToken);

			string uri = BaseUri();
			var dataRequest = await _httpClient.GetAsync(uri);

			string jsonContent = await dataRequest.Content.ReadAsStringAsync();
			ApiResponse<List<PublicChatGroupsDto>> apiResponse = JsonConvert
				.DeserializeObject<ApiResponse<List<PublicChatGroupsDto>>>(jsonContent)!;

			return apiResponse;
		}

		public async Task<ApiResponse<PublicChatGroupsDto>> PostNewGroup(CreatePublicChatGroupDto createDto, string jsonWebToken)
		{
			_httpClient = GetNewHttpClient(_httpClient, jsonWebToken);

			string baseUri = BaseUri();

			var bodyMessage = new StringContent(
				JsonConvert.SerializeObject(createDto), Encoding.UTF8, "application/json");

			var postRequest = await _httpClient.PostAsync(baseUri, bodyMessage);

			string jsonContent = await postRequest.Content.ReadAsStringAsync();
			ApiResponse<PublicChatGroupsDto> apiResponse = JsonConvert
				.DeserializeObject<ApiResponse<PublicChatGroupsDto>>(jsonContent)!;

			return apiResponse;
		}

		public async Task<ApiResponse<PublicChatGroupsDto>> UpdateGroup(ModifyPublicChatGroupDto modifyDto, string jsonWebToken)
		{
			_httpClient = GetNewHttpClient(_httpClient, jsonWebToken);

			string baseUri = BaseUri();

			var bodyMessage = new StringContent(
				JsonConvert.SerializeObject(modifyDto), Encoding.UTF8, "application/json");

			var updateRequest = await _httpClient.PutAsync(baseUri, bodyMessage);

			string jsonContent = await updateRequest.Content.ReadAsStringAsync();
			ApiResponse<PublicChatGroupsDto> apiResponse = JsonConvert
				.DeserializeObject<ApiResponse<PublicChatGroupsDto>>(jsonContent)!;

			return apiResponse;
		}

		public async Task<ApiResponse<PublicChatGroupsDto>> DeleteGroup(int groupId, string jsonWebToken)
		{
			_httpClient = GetNewHttpClient(_httpClient, jsonWebToken);

			var query = new Dictionary<string, string>()
			{
				["groupId"] = groupId.ToString()
			};

			string baseUri = BaseUri();
			var uriWithQuery = QueryHelpers.AddQueryString($"{baseUri}", query!);
			var deleteRequest = await _httpClient.DeleteAsync(uriWithQuery);

			if (!deleteRequest.IsSuccessStatusCode)
			{
				string jsonContent = await deleteRequest.Content.ReadAsStringAsync();
				ApiResponse<PublicChatGroupsDto> apiResponse = JsonConvert
					.DeserializeObject<ApiResponse<PublicChatGroupsDto>>(jsonContent)!;
				return apiResponse;
			}

			return new() { Success = true, Message = "ok" };
		}

		#region PRIVATE METHODS

		private string BaseUri()
		{
			return _configuration["ApiEndpointsConfig:PublicChatGroupsUri"]!;
			//return _configuration.GetValue<string>("ApiEndpointsConfig:PublicChatGroupsUri")!;
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
