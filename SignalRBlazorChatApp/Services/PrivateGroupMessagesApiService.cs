using ChatApplicationModels.Dtos;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using SignalRBlazorChatApp.HttpMethods;
using SignalRBlazorChatApp.Models;
using System.Text;

namespace SignalRBlazorChatApp.Services
{
	public class PrivateGroupMessagesApiService : IPrivateGroupMessagesApiService
    {
		private readonly IChatHttpMethods _httpMethods;

		public PrivateGroupMessagesApiService(IChatHttpMethods httpMethods)
        {
			_httpMethods = httpMethods ?? throw new Exception(nameof(httpMethods));
		}

        public async Task<ApiResponse<List<PrivateGroupMessageDto>>> GetMessagesByGroupId(
            int groupId, int numberMessagesToSkip, string jsonWebToken)
        {
            var query = new Dictionary<string, string>()
            {
                ["groupId"] = groupId.ToString(),
                ["numberMessagesToSkip"] = numberMessagesToSkip.ToString()
            };

            var pathWithQuery = QueryHelpers.AddQueryString("bygroupid", query!);

            var dataRequest = await _httpMethods.GetAsync(jsonWebToken, NamedHttpClients.PrivateMessageApi, pathWithQuery);

            string jsonContent = await dataRequest.Content.ReadAsStringAsync();

            ApiResponse<List<PrivateGroupMessageDto>> apiResponse = JsonConvert
                .DeserializeObject<ApiResponse<List<PrivateGroupMessageDto>>>(jsonContent)!;

            return apiResponse;
        }

        public async Task<ApiResponse<PrivateGroupMessageDto>> PostNewMessage(
            CreatePrivateGroupMessageDto createDto, string jsonWebToken)
        {
            var bodyMessage = new StringContent(
                JsonConvert.SerializeObject(createDto), Encoding.UTF8, "application/json");

            var postRequest = await _httpMethods.PostAsync(jsonWebToken, NamedHttpClients.PrivateMessageApi, bodyMessage);

            string jsonContent = await postRequest.Content.ReadAsStringAsync();

            ApiResponse<PrivateGroupMessageDto> apiResponse = JsonConvert
                .DeserializeObject<ApiResponse<PrivateGroupMessageDto>>(jsonContent)!;

            return apiResponse;
        }

        public async Task<ApiResponse<PrivateGroupMessageDto>> UpdateMessage(
            ModifyPrivateGroupMessageDto modifyDto, string jsonWebToken)
        {
            var bodyMessage = new StringContent(
                JsonConvert.SerializeObject(modifyDto), Encoding.UTF8, "application/json");

            var updateRequest = await _httpMethods.PutAsync(jsonWebToken, NamedHttpClients.PrivateMessageApi, bodyMessage);

            string jsonContent = await updateRequest.Content.ReadAsStringAsync();

            ApiResponse<PrivateGroupMessageDto> apiResponse = JsonConvert
                .DeserializeObject<ApiResponse<PrivateGroupMessageDto>>(jsonContent)!;

            return apiResponse;
        }

        public async Task<ApiResponse<PrivateGroupMessageDto>> DeleteMessage(Guid messageId, string jsonWebToken)
        {
            var query = new Dictionary<string, string>()
            {
                ["privateMessageId"] = messageId.ToString()
            };

            var pathWithQuery = QueryHelpers.AddQueryString("", query!);

            var deleteRequest = await _httpMethods.DeleteAsync(jsonWebToken, NamedHttpClients.PrivateMessageApi, pathWithQuery);

            if (!deleteRequest.IsSuccessStatusCode)
            {
                string jsonContent = await deleteRequest.Content.ReadAsStringAsync();

                ApiResponse<PrivateGroupMessageDto> apiResponse = JsonConvert
                    .DeserializeObject<ApiResponse<PrivateGroupMessageDto>>(jsonContent)!;

                return apiResponse;
            }

            return new() { Success = true, Message = "ok" };
        }
    }
}