using Moq;
using SignalRBlazorGroupsMessages.API.DataAccess;

namespace SignalRBlazorUnitTests.SignalRBlazorGroupMessage.API.UnitTests
{
    public class PublicMessageService_UnitTests
    {
        public PublicMessageService_UnitTests() 
        {
        }

        [Fact]
        public async Task GetByGroupIdAsync_ReturnsList()
        {
            var mockPublicMessageDataAccess = new Mock<IPublicMessagesDataAccess>().Object;

            var list = await mockPublicMessageDataAccess.GetViewListByGroupIdAsync(1, 0);
            //var list = _dataAccess.

            Assert.NotNull(list);
        }
    }
}
