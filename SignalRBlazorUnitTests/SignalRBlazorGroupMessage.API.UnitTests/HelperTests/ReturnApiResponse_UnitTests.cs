using SignalRBlazorGroupsMessages.API.Helpers;
using SignalRBlazorGroupsMessages.API.Models;

namespace SignalRBlazorUnitTests.SignalRBlazorGroupMessage.API.UnitTests.HelperTests
{
    public class ReturnApiResponse_UnitTests
    {
        [Fact]
        public void Success_ReturnsCorrectType()
        {
            int testType1 = 10;
            string testType2 = "test string";

            ApiResponse<int> apiResponse1 = new();
            ApiResponse<string> apiResponse2 = new();

            var result1 = ReturnApiResponse.Success(apiResponse1, testType1);
            var result2 = ReturnApiResponse.Success(apiResponse2, testType2);

            Assert.Multiple(() =>
            {
                Assert.True(result1.Success); 
                Assert.IsType<int>(result1.Data);

                Assert.True(result2.Success);
                Assert.IsType<string>(result2.Data);
            });
        }

        [Fact]
        public void Failure_ReturnsFalse()
        {
            ApiResponse<string> apiResponse1 = new();

            var result = ReturnApiResponse.Failure(apiResponse1, "test fail message");

            Assert.Multiple(() =>
            {
                Assert.False(result.Success);
                Assert.Null(result?.Data);
            });
        }
    }
}
