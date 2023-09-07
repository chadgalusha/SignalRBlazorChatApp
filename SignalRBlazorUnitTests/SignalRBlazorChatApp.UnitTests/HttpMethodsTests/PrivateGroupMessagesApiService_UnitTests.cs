using Moq;
using SignalRBlazorChatApp.HttpMethods;
using SignalRBlazorChatApp.Models;
using SignalRBlazorChatApp.Services;
using System.Net.Http.Formatting;
using System.Net;
using ChatApplicationModels.Dtos;

namespace SignalRBlazorUnitTests.SignalRBlazorChatApp.UnitTests.HttpMethodsTests
{
	public class PrivateGroupMessagesApiService_UnitTests
	{
		private Mock<IChatHttpMethods> _mockChatHttpMethods;

		public PrivateGroupMessagesApiService_UnitTests()
		{
			_mockChatHttpMethods = new Mock<IChatHttpMethods>();
		}

		[Fact]
		public async Task GetMessagesByGroupId_ReturnsTrue()
		{
			ApiResponse<List<PrivateGroupMessageDto>> apiResponse = new();
			string testToken = "";

			HttpResponseMessage httpResponseMessage = new()
			{
				StatusCode = HttpStatusCode.OK,
				Content = new ObjectContent<ApiResponse<List<PrivateGroupMessageDto>>>(
					apiResponse, new JsonMediaTypeFormatter(), "application/json")
			};

			_mockChatHttpMethods.Setup(g => g.GetAsync(testToken, NamedHttpClients.PrivateMessageApi, It.IsAny<string>()))
				.ReturnsAsync(httpResponseMessage);

			var service = GetNewService();

			var result = await service.GetMessagesByGroupId(1, 0, testToken);

			Assert.True(result.Success);
		}

		[Fact]
		public async Task GetMessagesByGroupId_ReturnsFalse()
		{
			ApiResponse<List<PrivateGroupMessageDto>> apiResponse = new()
			{
				Success = false,
				Message = "bad"
			};
			string testToken = "";

			HttpResponseMessage httpResponseMessage = new()
			{
				StatusCode = HttpStatusCode.BadRequest,
				Content = new ObjectContent<ApiResponse<List<PrivateGroupMessageDto>>>(
					apiResponse, new JsonMediaTypeFormatter(), "application/json")
			};

			_mockChatHttpMethods.Setup(g => g.GetAsync(testToken, NamedHttpClients.PrivateMessageApi, It.IsAny<string>()))
				.ReturnsAsync(httpResponseMessage);

			var service = GetNewService();

			var result = await service.GetMessagesByGroupId(1, 0, testToken);

			Assert.False(result.Success);
		}

		[Fact]
		public async Task PostNewMessage_ReturnsTrue()
		{
			ApiResponse<List<PrivateGroupMessageDto>> apiResponse = new();
			string testToken = "";

			HttpResponseMessage httpResponseMessage = new()
			{
				StatusCode = HttpStatusCode.OK,
				Content = new ObjectContent<ApiResponse<List<PrivateGroupMessageDto>>>(
					apiResponse, new JsonMediaTypeFormatter(), "application/json")
			};

			_mockChatHttpMethods.Setup(p => p.PostAsync(testToken, NamedHttpClients.PrivateMessageApi, It.IsAny<StringContent>()))
				.ReturnsAsync(httpResponseMessage);

			var service = GetNewService();

			var result = await service.PostNewMessage(new(), testToken);

			Assert.True(result.Success);
		}

		[Fact]
		public async Task PostNewMessage_ReturnsFalse()
		{
			ApiResponse<List<PrivateGroupMessageDto>> apiResponse = new()
			{
				Success = false,
				Message = "bad"
			};
			string testToken = "";

			HttpResponseMessage httpResponseMessage = new()
			{
				StatusCode = HttpStatusCode.BadRequest,
				Content = new ObjectContent<ApiResponse<List<PrivateGroupMessageDto>>>(
					apiResponse, new JsonMediaTypeFormatter(), "application/json")
			};

			_mockChatHttpMethods.Setup(p => p.PostAsync(testToken, NamedHttpClients.PrivateMessageApi, It.IsAny<StringContent>()))
				.ReturnsAsync(httpResponseMessage);

			var service = GetNewService();

			var result = await service.PostNewMessage(new(), testToken);

			Assert.False(result.Success);
		}

		[Fact]
		public async Task UpdateMessage_ReturnsTrue()
		{
			ApiResponse<List<PrivateGroupMessageDto>> apiResponse = new();
			string testToken = "";

			HttpResponseMessage httpResponseMessage = new()
			{
				StatusCode = HttpStatusCode.OK,
				Content = new ObjectContent<ApiResponse<List<PrivateGroupMessageDto>>>(
					apiResponse, new JsonMediaTypeFormatter(), "application/json")
			};

			_mockChatHttpMethods.Setup(p => p.PutAsync(testToken, NamedHttpClients.PrivateMessageApi, It.IsAny<StringContent>()))
				.ReturnsAsync(httpResponseMessage);

			var service = GetNewService();

			var result = await service.UpdateMessage(new(), testToken);

			Assert.True(result.Success);
		}

		[Fact]
		public async Task UpdateMessage_ReturnsFalse()
		{
			ApiResponse<List<PrivateGroupMessageDto>> apiResponse = new()
			{
				Success = false,
				Message = "bad"
			};
			string testToken = "";

			HttpResponseMessage httpResponseMessage = new()
			{
				StatusCode = HttpStatusCode.NotFound,
				Content = new ObjectContent<ApiResponse<List<PrivateGroupMessageDto>>>(
					apiResponse, new JsonMediaTypeFormatter(), "application/json")
			};

			_mockChatHttpMethods.Setup(p => p.PutAsync(testToken, NamedHttpClients.PrivateMessageApi, It.IsAny<StringContent>()))
				.ReturnsAsync(httpResponseMessage);

			var service = GetNewService();

			var result = await service.UpdateMessage(new(), testToken);

			Assert.False(result.Success);
		}

		[Fact]
		public async Task DeleteMessage_ReturnsTrue()
		{
			ApiResponse<List<PrivateGroupMessageDto>> apiResponse = new();
			string testToken = "";

			HttpResponseMessage httpResponseMessage = new()
			{
				StatusCode = HttpStatusCode.NoContent
			};

			_mockChatHttpMethods.Setup(d => d.DeleteAsync(testToken, NamedHttpClients.PrivateMessageApi, It.IsAny<string>()))
				.ReturnsAsync(httpResponseMessage);

			var service = GetNewService();

			var result = await service.DeleteMessage(It.IsAny<Guid>(), testToken);

			Assert.True(result.Success);
		}

		[Fact]
		public async Task DeleteMessage_ReturnsFalse()
		{
			ApiResponse<List<PrivateGroupMessageDto>> apiResponse = new()
			{
				Success = false,
				Message = "bad"
			};
			string testToken = "";

			HttpResponseMessage httpResponseMessage = new()
			{
				StatusCode = HttpStatusCode.NotFound,
				Content = new ObjectContent<ApiResponse<List<PrivateGroupMessageDto>>>(
					apiResponse, new JsonMediaTypeFormatter(), "application/json")
			};

			_mockChatHttpMethods.Setup(d => d.DeleteAsync(testToken, NamedHttpClients.PrivateMessageApi, It.IsAny<string>()))
				.ReturnsAsync(httpResponseMessage);

			var service = GetNewService();

			var result = await service.DeleteMessage(It.IsAny<Guid>(), testToken);

			Assert.False(result.Success);
		}

		#region PRIVATE METHODS

		private PrivateGroupMessagesApiService GetNewService()
		{
			return new(_mockChatHttpMethods.Object);
		}

		#endregion
	}
}
