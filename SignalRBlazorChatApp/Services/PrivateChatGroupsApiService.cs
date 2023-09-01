using ChatApplicationModels;
using ChatApplicationModels.Dtos;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using SignalRBlazorChatApp.HttpMethods;
using SignalRBlazorChatApp.Models;
using System.Text;

namespace SignalRBlazorChatApp.Services
{
	public class PrivateChatGroupsApiService : IPrivateChatGroupsApiService
    {
		private readonly IChatHttpMethods _httpMethods;

		public PrivateChatGroupsApiService(IChatHttpMethods httpMethods)
        {
			_httpMethods = httpMethods ?? throw new Exception(nameof(httpMethods));
		}

        public async Task<ApiResponse<List<PrivateChatGroupsDto>>> GetPrivateChatGroupsAsync(string userId, string jsonWebToken)
        {
            var query = new Dictionary<string, string>()
            {
                ["userId"] = userId
            };

            var pathWithQuery = QueryHelpers.AddQueryString($"byuserid", query!);

            var dataRequest = await _httpMethods.GetAsync(jsonWebToken, NamedHttpClients.PrivateGroupApi, pathWithQuery);

            string jsonContent = await dataRequest.Content.ReadAsStringAsync();

            ApiResponse<List<PrivateChatGroupsDto>> apiResponse = JsonConvert
                .DeserializeObject<ApiResponse<List<PrivateChatGroupsDto>>>(jsonContent)!;

            return apiResponse;
        }

        public async Task<ApiResponse<PrivateChatGroupsDto>> PostNewGroup(CreatePrivateChatGroupDto createDto, string jsonWebToken)
        {
            var bodyMessage = new StringContent(
                JsonConvert.SerializeObject(createDto), Encoding.UTF8, "application/json");

            var postRequest = await _httpMethods.PostAsync(jsonWebToken, NamedHttpClients.PrivateGroupApi, bodyMessage);

            string jsonContent = await postRequest.Content.ReadAsStringAsync();

            ApiResponse<PrivateChatGroupsDto> apiResponse = JsonConvert
                .DeserializeObject<ApiResponse<PrivateChatGroupsDto>>(jsonContent)!;

            return apiResponse;
        }

        public async Task<ApiResponse<PrivateChatGroupsDto>> UpdateGroup(ModifyPrivateChatGroupDto modifyDto, string jsonWebToken)
        {
            var bodyMessage = new StringContent(
                JsonConvert.SerializeObject(modifyDto), Encoding.UTF8, "application/json");

            var updateRequest = await _httpMethods.PutAsync(jsonWebToken, NamedHttpClients.PrivateGroupApi, bodyMessage);

			var jsonContent = await updateRequest.Content.ReadAsStringAsync();

            ApiResponse<PrivateChatGroupsDto> apiResponse = JsonConvert
                .DeserializeObject<ApiResponse<PrivateChatGroupsDto>>(jsonContent)!;

            return apiResponse;
        }

        public async Task<ApiResponse<PrivateChatGroupsDto>> DeleteGroup(int groupId, string jsonWebToken)
        {
            var query = new Dictionary<string, string>()
            {
                ["groupId"] = groupId.ToString()
            };

            var queryString = QueryHelpers.AddQueryString("", query!);

            var deleteRequest = await _httpMethods.DeleteAsync(jsonWebToken, NamedHttpClients.PrivateGroupApi, queryString);

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
            var query = new Dictionary<string, string>()
            {
                ["groupId"] = groupId.ToString(),
                ["userToAddId"] = userId
            };

            var pathWithQuery = QueryHelpers.AddQueryString($"groupmember", query!);

            var bodyMessage = new StringContent("", Encoding.UTF8, "application/json");

            var postRequest = await _httpMethods.PostAsync(jsonWebToken, NamedHttpClients.PrivateGroupApi, bodyMessage, pathWithQuery);

            var jsonContent = await postRequest.Content.ReadAsStringAsync();

            ApiResponse<PrivateGroupMembers> apiResponse = JsonConvert
                .DeserializeObject<ApiResponse<PrivateGroupMembers>>(jsonContent)!;

            return apiResponse;
        }

        public async Task<ApiResponse<PrivateGroupMembers>> DeleteGroupMember(int groupId, string userId, string jsonWebToken)
        {
            var query = new Dictionary<string, string>()
            {
                ["groupId"] = groupId.ToString(),
                ["userToAddId"] = userId
            };

            var pathWithQuery = QueryHelpers.AddQueryString("groupmember", query!);

            var deleteRequest = await _httpMethods.DeleteAsync(jsonWebToken, NamedHttpClients.PrivateGroupApi, pathWithQuery);

            if (!deleteRequest.IsSuccessStatusCode)
            {
                string jsonContent = await deleteRequest.Content.ReadAsStringAsync();

                ApiResponse<PrivateGroupMembers> apiResponse = JsonConvert
                    .DeserializeObject<ApiResponse<PrivateGroupMembers>>(jsonContent)!;

                return apiResponse;
            }

            return new() { Success = true, Message = "ok" };
        }
    }
}
