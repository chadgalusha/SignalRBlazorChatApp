using Moq;
using SignalRBlazorChatApp.HttpMethods;
using System.Net.Http.Formatting;
using System.Net;
using SignalRBlazorChatApp.Models;
using ChatApplicationModels.Dtos;
using SignalRBlazorChatApp.Services;

namespace SignalRBlazorUnitTests.SignalRBlazorChatApp.UnitTests.Services
{
	public class PrivateChatGroupsApiService_UnitTests
	{
		private Mock<IChatHttpMethods> _mockChatHttpMethods;

		public PrivateChatGroupsApiService_UnitTests()
		{
			_mockChatHttpMethods = new Mock<IChatHttpMethods>();
		}

		[Fact]
		public async Task GetPrivateChatGroupsAsync_ReturnsTrue()
		{
			ApiResponse<List<PrivateChatGroupsDto>> apiResponse = new();
			string testToken = "";

			HttpResponseMessage httpResponseMessage = new()
			{
				StatusCode = HttpStatusCode.OK,
				Content = new ObjectContent<ApiResponse<List<PrivateChatGroupsDto>>>(
					apiResponse, new JsonMediaTypeFormatter(), "application/json")
			};

			_mockChatHttpMethods.Setup(g => g.GetAsync(testToken, NamedHttpClients.PrivateGroupApi, It.IsAny<string>()))
				.ReturnsAsync(httpResponseMessage);

			var service = GetNewService();

			var result = await service.GetPrivateChatGroupsAsync(It.IsAny<string>(), testToken);

			Assert.True(result.Success);
		}

		[Fact]
		public async Task GetPrivateChatGroupsAsync_ReturnsFalse()
		{

			ApiResponse<List<PrivateChatGroupsDto>> apiResponse = new()
			{
				Success = false,
				Message = "bad"
			};
			string testToken = "";

			HttpResponseMessage httpResponseMessage = new()
			{
				StatusCode = HttpStatusCode.BadRequest,
				Content = new ObjectContent<ApiResponse<List<PrivateChatGroupsDto>>>(
					apiResponse, new JsonMediaTypeFormatter(), "application/json")
			};

			_mockChatHttpMethods.Setup(g => g.GetAsync(testToken, NamedHttpClients.PrivateGroupApi, It.IsAny<string>()))
				.ReturnsAsync(httpResponseMessage);

			var service = GetNewService();

			var result = await service.GetPrivateChatGroupsAsync(It.IsAny<string>(), testToken);

			Assert.False(result.Success);
		}

		[Fact]
		public async Task PostNewGroup_ReturnsTrue()
		{
			ApiResponse<List<PrivateChatGroupsDto>> apiResponse = new();
			string testToken = "";

			HttpResponseMessage httpResponseMessage = new()
			{
				StatusCode = HttpStatusCode.OK,
				Content = new ObjectContent<ApiResponse<List<PrivateChatGroupsDto>>>(
					apiResponse, new JsonMediaTypeFormatter(), "application/json")
			};

			_mockChatHttpMethods.Setup(p => p.PostAsync(testToken, NamedHttpClients.PrivateGroupApi, It.IsAny<StringContent>()))
				.ReturnsAsync(httpResponseMessage);

			var service = GetNewService();

			var result = await service.PostNewGroup(new(), testToken);

			Assert.True(result.Success);
		}

		[Fact]
		public async Task PostNewGroup_ReturnsFalse()
		{
			ApiResponse<List<PrivateChatGroupsDto>> apiResponse = new()
			{
				Success = false,
				Message = "bad"
			};
			string testToken = "";

			HttpResponseMessage httpResponseMessage = new()
			{
				StatusCode = HttpStatusCode.BadRequest,
				Content = new ObjectContent<ApiResponse<List<PrivateChatGroupsDto>>>(
					apiResponse, new JsonMediaTypeFormatter(), "application/json")
			};

			_mockChatHttpMethods.Setup(p => p.PostAsync(testToken, NamedHttpClients.PrivateGroupApi, It.IsAny<StringContent>()))
				.ReturnsAsync(httpResponseMessage);

			var service = GetNewService();

			var result = await service.PostNewGroup(new(), testToken);

			Assert.False(result.Success);
		}

		[Fact]
		public async Task UpdateGroup_ReturnsTrue()
		{
			ApiResponse<List<PrivateChatGroupsDto>> apiResponse = new();
			string testToken = "";

			HttpResponseMessage httpResponseMessage = new()
			{
				StatusCode = HttpStatusCode.OK,
				Content = new ObjectContent<ApiResponse<List<PrivateChatGroupsDto>>>(
					apiResponse, new JsonMediaTypeFormatter(), "application/json")
			};

			_mockChatHttpMethods.Setup(p => p.PutAsync(testToken, NamedHttpClients.PrivateGroupApi, It.IsAny<StringContent>()))
				.ReturnsAsync(httpResponseMessage);

			var service = GetNewService();

			var result = await service.UpdateGroup(new(), testToken);

			Assert.True(result.Success);
		}

		[Fact]
		public async Task UpdateGroup_ReturnsFalse()
		{
			ApiResponse<List<PrivateChatGroupsDto>> apiResponse = new()
			{
				Success = false,
				Message = "bad"
			};
			string testToken = "";

			HttpResponseMessage httpResponseMessage = new()
			{
				StatusCode = HttpStatusCode.NotFound,
				Content = new ObjectContent<ApiResponse<List<PrivateChatGroupsDto>>>(
					apiResponse, new JsonMediaTypeFormatter(), "application/json")
			};

			_mockChatHttpMethods.Setup(p => p.PutAsync(testToken, NamedHttpClients.PrivateGroupApi, It.IsAny<StringContent>()))
				.ReturnsAsync(httpResponseMessage);

			var service = GetNewService();

			var result = await service.UpdateGroup(new(), testToken);

			Assert.False(result.Success);
		}

		[Fact]
		public async Task DeleteGroup_ReturnsTrue()
		{
			ApiResponse<List<PrivateChatGroupsDto>> apiResponse = new();
			string testToken = "";

			HttpResponseMessage httpResponseMessage = new()
			{
				StatusCode = HttpStatusCode.NoContent
			};

			_mockChatHttpMethods.Setup(d => d.DeleteAsync(testToken, NamedHttpClients.PrivateGroupApi, It.IsAny<string>()))
				.ReturnsAsync(httpResponseMessage);

			var service = GetNewService();

			var result = await service.DeleteGroup(1, testToken);

			Assert.True(result.Success);
		}

		[Fact]
		public async Task DeleteGroup_ReturnsFalse()
		{
			ApiResponse<List<PrivateChatGroupsDto>> apiResponse = new()
			{
				Success = false,
				Message = "bad"
			};
			string testToken = "";

			HttpResponseMessage httpResponseMessage = new()
			{
				StatusCode = HttpStatusCode.NotFound,
				Content = new ObjectContent<ApiResponse<List<PrivateChatGroupsDto>>>(
					apiResponse, new JsonMediaTypeFormatter(), "application/json")
			};

			_mockChatHttpMethods.Setup(d => d.DeleteAsync(testToken, NamedHttpClients.PrivateGroupApi, It.IsAny<string>()))
				.ReturnsAsync(httpResponseMessage);

			var service = GetNewService();

			var result = await service.DeleteGroup(999, testToken);

			Assert.False(result.Success);
		}

		[Fact]
		public async Task PostGroupMember_ReturnsTrue()
		{
			ApiResponse<List<PrivateChatGroupsDto>> apiResponse = new();
			string testToken = "";

			HttpResponseMessage httpResponseMessage = new()
			{
				StatusCode = HttpStatusCode.OK,
				Content = new ObjectContent<ApiResponse<List<PrivateChatGroupsDto>>>(
					apiResponse, new JsonMediaTypeFormatter(), "application/json")
			};

			_mockChatHttpMethods.Setup(p => p.PostAsync(
					testToken, NamedHttpClients.PrivateGroupApi, It.IsAny<StringContent>(), It.IsAny<string>()))
				.ReturnsAsync(httpResponseMessage);

			var service = GetNewService();

			var result = await service.PostGroupMember(1, It.IsAny<string>(), testToken);

			Assert.True(result.Success);
		}

		[Fact]
		public async Task PostGroupMember_ReturnsFalse()
		{
			ApiResponse<List<PrivateChatGroupsDto>> apiResponse = new()
			{
				Success = false,
				Message = "bad"
			};
			string testToken = "";

			HttpResponseMessage httpResponseMessage = new()
			{
				StatusCode = HttpStatusCode.NotFound,
				Content = new ObjectContent<ApiResponse<List<PrivateChatGroupsDto>>>(
					apiResponse, new JsonMediaTypeFormatter(), "application/json")
			};

			_mockChatHttpMethods.Setup(p => p.PostAsync(
					testToken, NamedHttpClients.PrivateGroupApi, It.IsAny<StringContent>(), It.IsAny<string>()))
				.ReturnsAsync(httpResponseMessage);

			var service = GetNewService();

			var result = await service.PostGroupMember(1, It.IsAny<string>(), testToken);

			Assert.False(result.Success);
		}

		[Fact]
		public async Task DeleteGroupMember_ReturnsTrue()
		{
			ApiResponse<List<PrivateChatGroupsDto>> apiResponse = new();
			string testToken = "";

			HttpResponseMessage httpResponseMessage = new()
			{
				StatusCode = HttpStatusCode.OK,
				Content = new ObjectContent<ApiResponse<List<PrivateChatGroupsDto>>>(
					apiResponse, new JsonMediaTypeFormatter(), "application/json")
			};

			_mockChatHttpMethods.Setup(d => d.DeleteAsync(testToken, NamedHttpClients.PrivateGroupApi, It.IsAny<string>()))
				.ReturnsAsync(httpResponseMessage);

			var service = GetNewService();

			var result = await service.DeleteGroupMember(1, It.IsAny<string>(), testToken);

			Assert.True(result.Success);
		}

		[Fact]
		public async Task DeleteGroupMember_ReturnsFalse()
		{
			ApiResponse<List<PrivateChatGroupsDto>> apiResponse = new()
			{
				Success = false,
				Message = "bad"
			};
			string testToken = "";

			HttpResponseMessage httpResponseMessage = new()
			{
				StatusCode = HttpStatusCode.NotFound,
				Content = new ObjectContent<ApiResponse<List<PrivateChatGroupsDto>>>(
					apiResponse, new JsonMediaTypeFormatter(), "application/json")
			};

			_mockChatHttpMethods.Setup(d => d.DeleteAsync(testToken, NamedHttpClients.PrivateGroupApi, It.IsAny<string>()))
				.ReturnsAsync(httpResponseMessage);

			var service = GetNewService();

			var result = await service.DeleteGroupMember(1, It.IsAny<string>(), testToken);

			Assert.False(result.Success);
		}

		#region PRIVATE METHODS

		private PrivateChatGroupsApiService GetNewService()
		{
			return new(_mockChatHttpMethods.Object);
		}

		#endregion
	}
}
