using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System.Net;
using SignalRBlazorChatApp.Services;
using SignalRBlazorChatApp.HttpMethods;
using SignalRBlazorChatApp.Models;
using ChatApplicationModels.Dtos;
using Newtonsoft.Json;
using System.Net.Http.Formatting;

namespace SignalRBlazorUnitTests.SignalRBlazorChatApp.UnitTests.HttpMethodsTests
{
	public class PublicChatGroupsApiService_UnitTests
	{
		private Mock<IChatHttpMethods> _mockChatHttpMethods;

		public PublicChatGroupsApiService_UnitTests()
		{
			_mockChatHttpMethods = new Mock<IChatHttpMethods>();
		}

		[Fact]
		public async Task GetPublicChatGroupsAsync_ReturnsSuccess()
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

		#region PRIVATE METHODS

		private PublicChatGroupsApiService GetNewService()
		{
			return new(_mockChatHttpMethods.Object);
		}

		#endregion
	}
}
