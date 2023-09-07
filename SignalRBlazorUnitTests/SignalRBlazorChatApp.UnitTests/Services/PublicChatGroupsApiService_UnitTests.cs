using Moq;
using System.Net;
using SignalRBlazorChatApp.Services;
using SignalRBlazorChatApp.HttpMethods;
using SignalRBlazorChatApp.Models;
using ChatApplicationModels.Dtos;
using System.Net.Http.Formatting;

namespace SignalRBlazorUnitTests.SignalRBlazorChatApp.UnitTests.Services
{
	public class PublicChatGroupsApiService_UnitTests
	{
		private Mock<IChatHttpMethods> _mockChatHttpMethods;

		public PublicChatGroupsApiService_UnitTests()
		{
			_mockChatHttpMethods = new Mock<IChatHttpMethods>();
		}

		[Fact]
		public async Task GetPublicChatGroupsAsync_ReturnsTrue()
		{
			ApiResponse<List<PublicChatGroupsDto>> apiResponse = new();
			string testToken = "";

			HttpResponseMessage httpResponseMessage = new()
			{
				StatusCode = HttpStatusCode.OK,
				Content = new ObjectContent<ApiResponse<List<PublicChatGroupsDto>>>(
					apiResponse, new JsonMediaTypeFormatter(), "application/json")
			};

			_mockChatHttpMethods.Setup(g => g.GetAsync(testToken, NamedHttpClients.PublicGroupApi))
				.ReturnsAsync(httpResponseMessage);

			var service = GetNewService();

			var result = await service.GetPublicChatGroupsAsync(testToken);

			Assert.True(result.Success);
		}

		[Fact]
		public async Task GetPublicChatGroupsAsync_ReturnsFalse()
		{
			ApiResponse<List<PublicChatGroupsDto>> apiResponse = new()
			{
				Success = false,
				Message = "bad"
			};
			string testToken = "";

			HttpResponseMessage httpResponseMessage = new()
			{
				StatusCode = HttpStatusCode.BadRequest,
				Content = new ObjectContent<ApiResponse<List<PublicChatGroupsDto>>>(
					apiResponse, new JsonMediaTypeFormatter(), "application/json")
			};

			_mockChatHttpMethods.Setup(g => g.GetAsync(testToken, NamedHttpClients.PublicGroupApi))
				.ReturnsAsync(httpResponseMessage);

			var service = GetNewService();

			var result = await service.GetPublicChatGroupsAsync(testToken);

			Assert.False(result.Success);
		}

		[Fact]
		public async Task PostNewGroup_ReturnsTrue()
		{
			ApiResponse<List<PublicChatGroupsDto>> apiResponse = new();
			string testToken = "";

			HttpResponseMessage httpResponseMessage = new()
			{
				StatusCode = HttpStatusCode.OK,
				Content = new ObjectContent<ApiResponse<List<PublicChatGroupsDto>>>(
					apiResponse, new JsonMediaTypeFormatter(), "application/json")
			};

			_mockChatHttpMethods.Setup(p => p.PostAsync(testToken, NamedHttpClients.PublicGroupApi, It.IsAny<StringContent>()))
				.ReturnsAsync(httpResponseMessage);

			var service = GetNewService();

			var result = await service.PostNewGroup(new(), testToken);

			Assert.True(result.Success);
		}

		[Fact]
		public async Task PostNewGroup_ReturnsFalse()
		{
			ApiResponse<List<PublicChatGroupsDto>> apiResponse = new()
			{
				Success = false,
				Message = "bad"
			};
			string testToken = "";

			HttpResponseMessage httpResponseMessage = new()
			{
				StatusCode = HttpStatusCode.BadRequest,
				Content = new ObjectContent<ApiResponse<List<PublicChatGroupsDto>>>(
					apiResponse, new JsonMediaTypeFormatter(), "application/json")
			};

			_mockChatHttpMethods.Setup(p => p.PostAsync(testToken, NamedHttpClients.PublicGroupApi, It.IsAny<StringContent>()))
				.ReturnsAsync(httpResponseMessage);

			var service = GetNewService();

			var result = await service.PostNewGroup(new(), testToken);

			Assert.False(result.Success);
		}

		[Fact]
		public async Task UpdateGroup_ReturnsTrue()
		{
			ApiResponse<List<PublicChatGroupsDto>> apiResponse = new();
			string testToken = "";

			HttpResponseMessage httpResponseMessage = new()
			{
				StatusCode = HttpStatusCode.OK,
				Content = new ObjectContent<ApiResponse<List<PublicChatGroupsDto>>>(
					apiResponse, new JsonMediaTypeFormatter(), "application/json")
			};

			_mockChatHttpMethods.Setup(p => p.PutAsync(testToken, NamedHttpClients.PublicGroupApi, It.IsAny<StringContent>()))
				.ReturnsAsync(httpResponseMessage);

			var service = GetNewService();

			var result = await service.UpdateGroup(new(), testToken);

			Assert.True(result.Success);
		}

		[Fact]
		public async Task UpdateGroup_ReturnsFalse()
		{
			ApiResponse<List<PublicChatGroupsDto>> apiResponse = new()
			{
				Success = false,
				Message = "bad"
			};
			string testToken = "";

			HttpResponseMessage httpResponseMessage = new()
			{
				StatusCode = HttpStatusCode.NotFound,
				Content = new ObjectContent<ApiResponse<List<PublicChatGroupsDto>>>(
					apiResponse, new JsonMediaTypeFormatter(), "application/json")
			};

			_mockChatHttpMethods.Setup(p => p.PutAsync(testToken, NamedHttpClients.PublicGroupApi, It.IsAny<StringContent>()))
				.ReturnsAsync(httpResponseMessage);

			var service = GetNewService();

			var result = await service.UpdateGroup(new(), testToken);

			Assert.False(result.Success);
		}

		[Fact]
		public async Task DeleteGroup_ReturnsTrue()
		{
			ApiResponse<List<PublicChatGroupsDto>> apiResponse = new();
			string testToken = "";

			HttpResponseMessage httpResponseMessage = new()
			{
				StatusCode = HttpStatusCode.NoContent
			};

			_mockChatHttpMethods.Setup(d => d.DeleteAsync(testToken, NamedHttpClients.PublicGroupApi, It.IsAny<string>()))
				.ReturnsAsync(httpResponseMessage);

			var service = GetNewService();

			var result = await service.DeleteGroup(1, testToken);

			Assert.True(result.Success);
		}

		[Fact]
		public async Task DeleteGroup_ReturnsFalse()
		{
			ApiResponse<List<PublicChatGroupsDto>> apiResponse = new()
			{
				Success = false,
				Message = "bad"
			};
			string testToken = "";

			HttpResponseMessage httpResponseMessage = new()
			{
				StatusCode = HttpStatusCode.NotFound,
				Content = new ObjectContent<ApiResponse<List<PublicChatGroupsDto>>>(
					apiResponse, new JsonMediaTypeFormatter(), "application/json")
			};

			_mockChatHttpMethods.Setup(d => d.DeleteAsync(testToken, NamedHttpClients.PublicGroupApi, It.IsAny<string>()))
				.ReturnsAsync(httpResponseMessage);

			var service = GetNewService();

			var result = await service.DeleteGroup(999, testToken);

			Assert.False(result.Success);
		}

		#region PRIVATE METHODS

		private PublicChatGroupsApiService GetNewService()
		{
			return new(_mockChatHttpMethods.Object);
		}

		#endregion
	}
}
