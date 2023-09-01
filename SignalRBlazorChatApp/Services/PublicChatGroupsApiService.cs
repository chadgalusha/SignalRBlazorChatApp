using ChatApplicationModels.Dtos;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using SignalRBlazorChatApp.HttpMethods;
using SignalRBlazorChatApp.Models;
using System.Text;

namespace SignalRBlazorChatApp.Services
{
    public class PublicChatGroupsApiService : IPublicChatGroupsApiService
    {
        private readonly IChatHttpMethods _httpMethods;

        public PublicChatGroupsApiService(IChatHttpMethods httpMethods)
        {
            _httpMethods = httpMethods ?? throw new Exception(nameof(httpMethods));
        }

        public async Task<ApiResponse<List<PublicChatGroupsDto>>> GetPublicChatGroupsAsync(string jsonWebToken)
        {
            var dataRequest = await _httpMethods.GetAsync(jsonWebToken, NamedHttpClients.PublicGroupApi);

            string jsonContent = await dataRequest.Content.ReadAsStringAsync();

            ApiResponse<List<PublicChatGroupsDto>> apiResponse = JsonConvert
                .DeserializeObject<ApiResponse<List<PublicChatGroupsDto>>>(jsonContent)!;

            return apiResponse;
        }

        public async Task<ApiResponse<PublicChatGroupsDto>> PostNewGroup(CreatePublicChatGroupDto createDto, string jsonWebToken)
        {
            var bodyMessage = new StringContent(
                JsonConvert.SerializeObject(createDto), Encoding.UTF8, "application/json");

            var postRequest = await _httpMethods.PostAsync(jsonWebToken, NamedHttpClients.PublicGroupApi, bodyMessage);

            string jsonContent = await postRequest.Content.ReadAsStringAsync();

            ApiResponse<PublicChatGroupsDto> apiResponse = JsonConvert
                .DeserializeObject<ApiResponse<PublicChatGroupsDto>>(jsonContent)!;

            return apiResponse;
        }

        public async Task<ApiResponse<PublicChatGroupsDto>> UpdateGroup(ModifyPublicChatGroupDto modifyDto, string jsonWebToken)
        {
            var bodyMessage = new StringContent(
                JsonConvert.SerializeObject(modifyDto), Encoding.UTF8, "application/json");

            var updateRequest = await _httpMethods.PutAsync(jsonWebToken, NamedHttpClients.PublicGroupApi, bodyMessage);

            string jsonContent = await updateRequest.Content.ReadAsStringAsync();

            ApiResponse<PublicChatGroupsDto> apiResponse = JsonConvert
                .DeserializeObject<ApiResponse<PublicChatGroupsDto>>(jsonContent)!;

            return apiResponse;
        }

        public async Task<ApiResponse<PublicChatGroupsDto>> DeleteGroup(int groupId, string jsonWebToken)
        {
            var query = new Dictionary<string, string>()
            {
                ["groupId"] = groupId.ToString()
            };

            var queryString = QueryHelpers.AddQueryString("", query!);

            var deleteRequest = await _httpMethods.DeleteAsync(jsonWebToken, NamedHttpClients.PublicGroupApi, queryString);

            if (!deleteRequest.IsSuccessStatusCode)
            {
                string jsonContent = await deleteRequest.Content.ReadAsStringAsync();

                ApiResponse<PublicChatGroupsDto> apiResponse = JsonConvert
                    .DeserializeObject<ApiResponse<PublicChatGroupsDto>>(jsonContent)!;

                return apiResponse;
            }

            return new() { Success = true, Message = "ok" };
        }
    }
}
