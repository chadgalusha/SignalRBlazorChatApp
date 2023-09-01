using ChatApplicationModels.Dtos;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using SignalRBlazorChatApp.HttpMethods;
using SignalRBlazorChatApp.Models;
using System.Text;

namespace SignalRBlazorChatApp.Services
{
	public class PublicGroupMessagesApiService : IPublicGroupMessagesApiService
    {
		private readonly IChatHttpMethods _httpMethods;

		public PublicGroupMessagesApiService(IChatHttpMethods httpMethods)
        {
			_httpMethods = httpMethods ?? throw new Exception(nameof(httpMethods));
		}

        public async Task<ApiResponse<List<PublicGroupMessageDto>>> GetMessagesByGroupId(
            int groupId, int numberMessagesToSkip, string jsonWebToken)
        {
            var query = new Dictionary<string, string>()
            {
                ["groupId"] = groupId.ToString(),
                ["numberMessagesToSkip"] = numberMessagesToSkip.ToString()
            };

            var pathWithQuery = QueryHelpers.AddQueryString("bygroupid", query!);

            var dataRequest = await _httpMethods.GetAsync(jsonWebToken, NamedHttpClients.PublicMessageApi, pathWithQuery);

            string jsonContent = await dataRequest.Content.ReadAsStringAsync();

            ApiResponse<List<PublicGroupMessageDto>> apiResponse = JsonConvert
                .DeserializeObject<ApiResponse<List<PublicGroupMessageDto>>>(jsonContent)!;

            return apiResponse;
        }

        public async Task<ApiResponse<PublicGroupMessageDto>> PostNewMessage(
            CreatePublicGroupMessageDto createDto, string jsonWebToken)
        {
            var bodyMessage = new StringContent(
                JsonConvert.SerializeObject(createDto), Encoding.UTF8, "application/json");

            var postRequest = await _httpMethods.PostAsync(jsonWebToken, NamedHttpClients.PublicMessageApi, bodyMessage);

            string jsonContent = await postRequest.Content.ReadAsStringAsync();

            ApiResponse<PublicGroupMessageDto> apiResponse = JsonConvert
                .DeserializeObject<ApiResponse<PublicGroupMessageDto>>(jsonContent)!;

            return apiResponse;
        }

        public async Task<ApiResponse<PublicGroupMessageDto>> UpdateMessage(
            ModifyPublicGroupMessageDto modifyDto, string jsonWebToken)
        {
            var bodyMessage = new StringContent(
                JsonConvert.SerializeObject(modifyDto), Encoding.UTF8, "application/json");

            var updateRequest = await _httpMethods.PutAsync(jsonWebToken, NamedHttpClients.PublicMessageApi, bodyMessage);

            string jsonContent = await updateRequest.Content.ReadAsStringAsync();

            ApiResponse<PublicGroupMessageDto> apiResponse = JsonConvert
                .DeserializeObject<ApiResponse<PublicGroupMessageDto>>(jsonContent)!;

            return apiResponse;
        }

        public async Task<ApiResponse<PublicGroupMessageDto>> DeleteMessage(Guid messageId, string jsonWebToken)
        {
            var query = new Dictionary<string, string>()
            {
                ["messageId"] = messageId.ToString()
            };

            var queryString = QueryHelpers.AddQueryString("", query!);

            var deleteRequest = await _httpMethods.DeleteAsync(jsonWebToken, NamedHttpClients.PublicMessageApi, queryString);

            if (!deleteRequest.IsSuccessStatusCode)
            {
                string jsonContent = await deleteRequest.Content.ReadAsStringAsync();

                ApiResponse<PublicGroupMessageDto> apiResponse = JsonConvert
                    .DeserializeObject<ApiResponse<PublicGroupMessageDto>>(jsonContent)!;

                return apiResponse;
            }

            return new() { Success = true, Message = "ok" };
        }
    }
}