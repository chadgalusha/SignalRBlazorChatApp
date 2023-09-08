using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using SignalRBlazorChatApp.HttpMethods;
using System.Net;
using System.Text;

namespace SignalRBlazorUnitTests.SignalRBlazorChatApp.UnitTests.HttpMethods
{
	public class ChatHttpMethods_UnitTests
	{
		private readonly Mock<IHttpClientFactory> _mockClientFactory;

		public ChatHttpMethods_UnitTests()
		{
			_mockClientFactory = new Mock<IHttpClientFactory>();
		}

		[Fact]
		public async Task GetAsync_TwoParams_ReturnsContent()
		{
			// Arrange
			string testToken = "test";
			string testClient = "test";

			var mockHttpClientObject = GetMockClientObject();

			_mockClientFactory.Setup(c => c.CreateClient(testClient))
				.Returns(mockHttpClientObject);

			var chatHttpMethods = GetNewChatHttpMethods();

			// Act
			var result = await chatHttpMethods.GetAsync(testToken, testClient);
			
		    // Assert
			Assert.Equal(HttpStatusCode.OK, result.StatusCode);
		}

		[Fact]
		public async Task GetAsync_ThreeParams_ReturnsContent()
		{
			// Arrange
			string testToken = "test";
			string testClient = "test";
			string testPath = "";

			var mockHttpClientObject = GetMockClientObject();

			_mockClientFactory.Setup(c => c.CreateClient(testClient))
				.Returns(mockHttpClientObject);

			var chatHttpMethods = GetNewChatHttpMethods();

			// Act
			var result = await chatHttpMethods.GetAsync(testToken, testClient, testPath);

			// Assert
			Assert.Equal(HttpStatusCode.OK, result.StatusCode);
		}

		[Fact]
		public async Task PostAsync_ThreeParams_ReturnsContent()
		{
			// Arrange
			string testToken = "test";
			string testClient = "test";
			StringContent testContent = GetTestStringContent();

			var testHttpClient = PostPutTestClient();

			_mockClientFactory.Setup(c => c.CreateClient(testClient))
				.Returns(testHttpClient);

			var chatHttpMethods = GetNewChatHttpMethods();

			// Act
			var result = await chatHttpMethods.PostAsync(testToken, testClient, testContent);

			// Assert
			Assert.Equal(HttpStatusCode.OK, result.StatusCode);
		}

		[Fact]
		public async Task PostAsync_FourParams_ReturnsContent()
		{
			// Arrange
			string testToken = "test";
			string testClient = "test";
			StringContent testContent = GetTestStringContent();
			string path = "";

			var testHttpClient = PostPutTestClient();

			_mockClientFactory.Setup(c => c.CreateClient(testClient))
				.Returns(testHttpClient);

			var chatHttpMethods = GetNewChatHttpMethods();

			// Act
			var result = await chatHttpMethods.PostAsync(testToken, testClient, testContent, path);

			// Assert
			Assert.Equal(HttpStatusCode.OK, result.StatusCode);
		}

		[Fact]
		public async Task PutAsync_ReturnsContent()
		{
			// Arrange
			string testToken = "test";
			string testClient = "test";
			StringContent testContent = GetTestStringContent();

			var testHttpClient = PostPutTestClient();

			_mockClientFactory.Setup(c => c.CreateClient(testClient))
				.Returns(testHttpClient);

			var chatHttpMethods = GetNewChatHttpMethods();

			// Act
			var result = await chatHttpMethods.PutAsync(testToken, testClient, testContent);

			// Assert
			Assert.Equal(HttpStatusCode.OK, result.StatusCode);
		}

		[Fact]
		public async Task DeleteAsync_ReturnsNoContent()
		{
			// Arrange
			string testToken = "test";
			string testClient = "test";
			string testQuery = "test";

			var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
			mockHttpMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.NoContent });

			var testHttpClient = new HttpClient(mockHttpMessageHandler.Object);
			testHttpClient.BaseAddress = new Uri("https://www.example.com");

			_mockClientFactory.Setup(c => c.CreateClient(testClient))
				.Returns(testHttpClient);

			var chatHttpMethods = GetNewChatHttpMethods();

			// Act
			var result = await chatHttpMethods.DeleteAsync(testToken, testClient, testQuery);

			// Assert
			Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
		}

		#region PRIVATE METHODS

		private ChatHttpMethods GetNewChatHttpMethods()
		{
			return new(_mockClientFactory.Object);
		}

		private HttpClient GetMockClientObject()
		{
			var mockHttpClient = new Mock<HttpClient>();
			var mockObject = mockHttpClient.Object;
			mockObject.BaseAddress = new Uri("https://www.example.com");
			return mockObject;
		}

		private HttpClient PostPutTestClient()
		{
			var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
			mockHttpMessageHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK });

			HttpClient testHttpClient = new(mockHttpMessageHandler.Object)
			{
				BaseAddress = new Uri("https://www.example.com")
			};

			return testHttpClient;
		}

		private StringContent GetTestStringContent()
		{
			return new StringContent(
				JsonConvert.SerializeObject("Test"), Encoding.UTF8, "application/json");
		}

		#endregion
	}
}
