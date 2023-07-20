using Microsoft.Extensions.Configuration;
using Moq;
using SignalRBlazorGroupsMessages.API.DataAccess;
using SignalRBlazorGroupsMessages.API.Models.Dtos;
using static SignalRBlazorUnitTests.SignalRBlazorGroupMessage.API.UnitTests.PrivateMessagesTests.PrivateGroupMessagesDatabaseFixture;

namespace SignalRBlazorUnitTests.SignalRBlazorGroupMessage.API.UnitTests.PrivateMessagesTests
{
    public class PrivateGroupMessagesDataAccess_UnitTests : IClassFixture<PrivateGroupMessagesDatabaseFixture>
    {
        public PrivateGroupMessagesDatabaseFixture Fixture { get; }
        private readonly PrivateMessagesDataAccess _dataAccess;
        private readonly TestPrivateGroupMessagesDbContext _context;
        private readonly IConfiguration _configuration;

        public PrivateGroupMessagesDataAccess_UnitTests(PrivateGroupMessagesDatabaseFixture fixture)
        {
            Fixture = fixture;
            _context = Fixture.CreateContext();
            _configuration = new Mock<IConfiguration>().Object;
            _dataAccess = new PrivateMessagesDataAccess(_context, _configuration);
        }

        [Fact]
        public async Task GetDtoListByGroupIdAsync_ReturnsMessages()
        {
            int groupId = 1;
            List<PrivateGroupMessageDto> dtoList = _context.PrivateMessagesDto
                .Where(p => p.ChatGroupId == groupId)
                .ToList();

            Mock<IPrivateMessagesDataAccess> _mockPrivateMessagesDataAccess = new();
            _mockPrivateMessagesDataAccess.Setup(p => p.GetDtoListByGroupIdAsync(groupId, 0))
                .ReturnsAsync(dtoList);

            var mockedDataAccess = _mockPrivateMessagesDataAccess.Object;
            List<PrivateGroupMessageDto> result = await mockedDataAccess.GetDtoListByGroupIdAsync(groupId, 0);

            Assert.Multiple(() =>
            {
                Assert.NotNull(result);
                Assert.Equal(dtoList, result);
            });
        }

        [Fact]
        public async Task GetDtoListByUserIdAsync_ReturnsMessages()
        {
            string userId = "e1b9cf9a-ff86-4607-8765-9e47a305062a";
            List<PrivateGroupMessageDto> dtoList = _context.PrivateMessagesDto
                .Where(p => p.UserId == userId)
                .ToList();

            Mock<IPrivateMessagesDataAccess> _mockPrivateMessagesDataAccess = new();
            _mockPrivateMessagesDataAccess.Setup(p => p.GetDtoListByUserIdAsync(userId, 0))
                .ReturnsAsync(dtoList);

            var mockedDataAccess = _mockPrivateMessagesDataAccess.Object;
            List<PrivateGroupMessageDto> result = await mockedDataAccess.GetDtoListByUserIdAsync(userId, 0);

            Assert.Multiple(() =>
            {
                Assert.NotNull(result);
                Assert.Equal(dtoList, result);
            });
        }

        [Fact]
        public async Task GetDtoByMessageIdAsync_ReturnsMessage()
        {
           Guid messageId = Guid.Parse("e8ee70b6-678a-4b86-934e-da7f404a33a3");
            PrivateGroupMessageDto dto = _context.PrivateMessagesDto
                 .Single(p => p.PrivateMessageId == messageId);

            Mock<IPrivateMessagesDataAccess> _mockPrivateMessagesDataAccess = new();
            _mockPrivateMessagesDataAccess.Setup(p => p.GetDtoByMessageIdAsync(messageId))
                .ReturnsAsync(dto);

            var mockedDataAccess = _mockPrivateMessagesDataAccess.Object;
            PrivateGroupMessageDto result = await mockedDataAccess.GetDtoByMessageIdAsync(messageId);

            Assert.Multiple(() =>
            {
                Assert.NotNull(result);
                Assert.Equal(dto, result);
            });
        }

    }
}
