using ChatApplicationModels.Dtos;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using SignalRBlazorChatApp.HttpMethods;
using System.Net;
using SignalRBlazorGroupsMessages.API.Models;
using System.Reflection;

namespace SignalRBlazorUnitTests.SignalRBlazorChatApp.UnitTests.HttpMethodsTests
{
	public class PublicChatGroupsApiService_UnitTests
	{
		private Mock<HttpClient> _mockHttpClient;
		private readonly Mock<IConfiguration> _mockIConfiguration;

		public PublicChatGroupsApiService_UnitTests()
		{
			_mockHttpClient = new Mock<HttpClient>();
			_mockIConfiguration = new Mock<IConfiguration>();
		}

		[Fact]
		public async Task GetPublicChatGroupsAsync_ReturnsSuccess()
		{
			Uri baseUri = new("https://localhost:7151");

			var mockMessageHandler = new Mock<HttpMessageHandler>();
			mockMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{ 
					StatusCode = HttpStatusCode.OK,
					Content = new StringContent("test content")
				});

			HttpClient client = new(mockMessageHandler.Object);
			//{
			//	BaseAddress = baseUri
			//};

			//var mockApiEndpoints = new Dictionary<string, string>
			//{
			//	{"ApiEndpointsConfig:PublicChatGroupsUri", "TestEndpoint" }
			//};
			//var configSection = new Mock<IConfigurationSection>();
			//configSection.Setup(x => x.Value).Returns(baseUri.ToString());

			//_mockIConfiguration.Setup(x => x.GetSection("ApiEndpointsConfig:PublicChatGroupsUri")).Returns(configSection.Object);

			//var configObject = new ConfigurationBuilder()
			//	.AddInMemoryCollection(mockApiEndpoints)
			//	.Build();

			//_mockIConfiguration.Setup(g => g.GetValue<string>(It.IsAny<string>())).Returns(baseUri.AbsoluteUri);
			_mockIConfiguration.SetupGet(x => x[It.IsAny<string>()]).Returns(baseUri.AbsoluteUri);

			PublicChatGroupsApiService _mockService = new(client, _mockIConfiguration.Object);

			var result = await _mockService.GetPublicChatGroupsAsync("testToken");

			Assert.NotNull(result);
			Assert.True(result.Success);
			// possible solutions
			// https://github.com/maxkagamine/Moq.Contrib.HttpClient
			// https://stackoverflow.com/questions/36425008/mocking-httpclient-in-unit-tests
		}

		#region PRIVATE METHODS

		private PublicChatGroupsApiService GetNewService()
		{
			return new(_mockHttpClient.Object, _mockIConfiguration.Object);
		}

		#endregion
	}
}
