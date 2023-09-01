using ChatApplicationModels;
using ChatApplicationModels.Dtos;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using SignalRBlazorChatApp.Models;
using System.Net.Http.Headers;
using System.Text;

namespace SignalRBlazorChatApp.Services
{
    public class PrivateChatGroupsApiService : IPrivateChatGroupsApiService
    {
        private HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public PrivateChatGroupsApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient ?? throw new Exception(nameof(httpClient));
            _configuration = configuration ?? throw new Exception(nameof(configuration));
        }

        public async Task<ApiResponse<List<PrivateChatGroupsDto>>> GetPrivateChatGroupsAsync(string userId, string jsonWebToken)
        {
            _httpClient = GetNewHttpClient(_httpClient, jsonWebToken);

            var query = new Dictionary<string, string>()
            {
                ["userId"] = userId
            };

            string uri = BaseUri();
            var uriWithQuery = QueryHelpers.AddQueryString($"{uri}byuserid", query!);
            var dataRequest = await _httpClient.GetAsync(uriWithQuery);

            string jsonContent = await dataRequest.Content.ReadAsStringAsync();
            ApiResponse<List<PrivateChatGroupsDto>> apiResponse = JsonConvert
                .DeserializeObject<ApiResponse<List<PrivateChatGroupsDto>>>(jsonContent)!;

            return apiResponse;
        }

        public async Task<ApiResponse<PrivateChatGroupsDto>> PostNewGroup(CreatePrivateChatGroupDto createDto, string jsonWebToken)
        {
            _httpClient = GetNewHttpClient(_httpClient, jsonWebToken);

            string baseUri = BaseUri();

            var bodyMessage = new StringContent(
                JsonConvert.SerializeObject(createDto), Encoding.UTF8, "application/json");

            var postRequest = await _httpClient.PostAsync(baseUri, bodyMessage);

            string jsonContent = await postRequest.Content.ReadAsStringAsync();
            ApiResponse<PrivateChatGroupsDto> apiResponse = JsonConvert
                .DeserializeObject<ApiResponse<PrivateChatGroupsDto>>(jsonContent)!;

            return apiResponse;
        }

        public async Task<ApiResponse<PrivateChatGroupsDto>> UpdateGroup(ModifyPrivateChatGroupDto modifyDto, string jsonWebToken)
        {
            _httpClient = GetNewHttpClient(_httpClient, jsonWebToken);

            string baseUri = BaseUri();

            var bodyMessage = new StringContent(
                JsonConvert.SerializeObject(modifyDto), Encoding.UTF8, "application/json");

            var updateRequest = await _httpClient.PutAsync(baseUri, bodyMessage);

            var jsonContent = await updateRequest.Content.ReadAsStringAsync();
            ApiResponse<PrivateChatGroupsDto> apiResponse = JsonConvert
                .DeserializeObject<ApiResponse<PrivateChatGroupsDto>>(jsonContent)!;

            return apiResponse;
        }

        public async Task<ApiResponse<PrivateChatGroupsDto>> DeleteGroup(int groupId, string jsonWebToken)
        {
            _httpClient = GetNewHttpClient(_httpClient, jsonWebToken);

            var query = new Dictionary<string, string>()
            {
                ["groupId"] = groupId.ToString()
            };

            string baseUri = BaseUri();
            var uriWithQuery = QueryHelpers.AddQueryString(baseUri, query!);

            var deleteRequest = await _httpClient.DeleteAsync(uriWithQuery);

            if (!deleteRequest.IsSuccessStatusCode)
            {
                string jsonContent = await deleteRequest.Content.ReadAsStringAsync();
                ApiResponse<PrivateChatGroupsDto> apiResponse = JsonConvert
                    .DeserializeObject<ApiResponse<PrivateChatGroupsDto>>(jsonContent)!;
                return apiResponse;
            }

            return new() { Success = true, Message = "ok" };
        }

        public async Task<ApiResponse<PrivateGroupMembers>> PostGroupMember(int groupId, string userId, string jsonWebToken)
        {
            _httpClient = GetNewHttpClient(_httpClient, jsonWebToken);

            var query = new Dictionary<string, string>()
            {
                ["groupId"] = groupId.ToString(),
                ["userToAddId"] = userId
            };

            string baseUri = BaseUri();
            var uriWithQuery = QueryHelpers.AddQueryString($"{baseUri}groupmember", query!);

            var bodyMessage = new StringContent("", Encoding.UTF8, "application/json");

            var postRequest = await _httpClient.PostAsync(uriWithQuery, bodyMessage);

            var jsonContent = await postRequest.Content.ReadAsStringAsync();
            ApiResponse<PrivateGroupMembers> apiResponse = JsonConvert
                .DeserializeObject<ApiResponse<PrivateGroupMembers>>(jsonContent)!;

            return apiResponse;
        }

        public async Task<ApiResponse<PrivateGroupMembers>> DeleteGroupMember(int groupId, string userId, string jsonWebToken)
        {
            _httpClient = GetNewHttpClient(_httpClient, jsonWebToken);

            var query = new Dictionary<string, string>()
            {
                ["groupId"] = groupId.ToString(),
                ["userToAddId"] = userId
            };

            string baseUri = BaseUri();
            var uriWithQuery = QueryHelpers.AddQueryString($"{baseUri}groupmember", query!);

            var deleteRequest = await _httpClient.DeleteAsync(uriWithQuery);

            if (!deleteRequest.IsSuccessStatusCode)
            {
                string jsonContent = await deleteRequest.Content.ReadAsStringAsync();
                ApiResponse<PrivateGroupMembers> apiResponse = JsonConvert
                    .DeserializeObject<ApiResponse<PrivateGroupMembers>>(jsonContent)!;
                return apiResponse;
            }

            return new() { Success = true, Message = "ok" };
        }

        #region PRIVATE METHODS

        private string BaseUri()
        {
            return _configuration["ApiEndpointsConfig:PrivateChatGroupsUri"]!;
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
