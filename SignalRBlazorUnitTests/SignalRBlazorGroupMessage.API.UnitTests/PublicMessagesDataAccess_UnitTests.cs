using ChatApplicationModels;
using SignalRBlazorGroupsMessages.API.DataAccess;

namespace SignalRBlazorUnitTests.SignalRBlazorGroupMessage.API.UnitTests
{
    public class PublicMessagesDataAccess_UnitTests : IClassFixture<TestPublicMessagesDatabaseFixture>
    {
        public TestPublicMessagesDatabaseFixture Fixture { get; }

        public PublicMessagesDataAccess_UnitTests(TestPublicMessagesDatabaseFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public async Task GetPublicMessages_ReturnsMessagesAsync()
        {
            using var context = Fixture.CreateContext();
            var dataAccess = new PublicMessagesDataAccess(context);

            List<PublicMessages> listMessages = await dataAccess.GetMessagesByGroupAsync(2);

            Assert.NotNull(listMessages);
        }
    }
}
