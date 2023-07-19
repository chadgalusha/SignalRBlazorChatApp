using ChatApplicationModels;
using Moq;
using SignalRBlazorGroupsMessages.API.DataAccess;
using SignalRBlazorGroupsMessages.API.Helpers;
using SignalRBlazorGroupsMessages.API.Models.Dtos;
using SignalRBlazorGroupsMessages.API.Services;

namespace SignalRBlazorUnitTests.SignalRBlazorGroupMessage.API.UnitTests.PublicMessages
{
    public class PublicMessageService_UnitTests
    {
        private readonly Mock<IPublicMessagesDataAccess> _mockDataAccess;
        private readonly Mock<ISerilogger> _mockSerilogger;

        public PublicMessageService_UnitTests()
        {
            _mockDataAccess = new Mock<IPublicMessagesDataAccess>();
            _mockSerilogger = new Mock<ISerilogger>();
        }

        [Fact]
        public async Task GetListByGroupIdAsync_ReturnsCorrectResults()
        {
            int expectedCount = GetListPublicMessagesDto()
                .Where(p => p.ChatGroupId == 1)
                .ToList()
                .Count;

            _mockDataAccess.Setup(m => m.GetDtoListByGroupIdAsync(1, 0))
                .ReturnsAsync(GetListPublicMessagesDto()
                    .Where(x => x.ChatGroupId == 1)
                    .ToList());

            PublicMessagesService _service = GetNewService();

            var result1 = await _service.GetListByGroupIdAsync(1, 0);
            var result2 = await _service.GetListByGroupIdAsync(999, 0);

            Assert.Multiple(() =>
            {
                Assert.Equal(expectedCount, result1.Data?.Count);
                Assert.True(result1.Success == true);
                Assert.True(result1.Message == "ok");
                // As groupId 999 not valid, result2.Data should be set to null.
                Assert.Null(result2.Data);
            });
        }

        [Fact]
        public async Task GetListByUserIdAsync_ReturnsCorrectResults()
        {
            string testUserId = "e8ee70b6-678a-4b86-934e-da7f404a33a3";
            int expectedCount = GetListPublicMessagesDto()
                .Where(u => u.UserId == testUserId)
                .ToList()
                .Count;

            _mockDataAccess.Setup(m => m.GetDtoListByUserIdAsync(testUserId, 0))
                .ReturnsAsync(GetListPublicMessagesDto()
                    .Where(u => u.UserId == testUserId)
                    .ToList());

            PublicMessagesService _service = GetNewService();

            var result1 = await _service.GetListByUserIdAsync(testUserId, 0);

            Assert.Multiple(() =>
            {
                Assert.Equal(expectedCount, result1.Data?.Count);
                Assert.True(result1.Success);
            });
        }

        [Fact]
        public async Task GetByMessageIdAsync_ReturnsMessage()
        {
            Guid testMessageId = Guid.Parse("e8ee70b6-678a-4b86-934e-da7f404a33a3");
            PublicGroupMessageDto dtoMessage = GetListPublicMessagesDto()
                .Single(g => g.PublicMessageId == testMessageId);

            _mockDataAccess.Setup(p => p.GetDtoByMessageIdAsync(testMessageId))
                .ReturnsAsync(GetListPublicMessagesDto()
                    .Single(m => m.PublicMessageId == testMessageId));

            PublicMessagesService _service = GetNewService();

            var result = await _service.GetByMessageIdAsync(testMessageId);

            Assert.Multiple(() =>
            {
                Assert.NotNull(result.Data);
                Assert.True(result.Success);
                Assert.Equal("TestUser1", result.Data.UserName);
            });
        }

        [Fact]
        public async Task AddAsync_IsSuccess()
        {
            PublicGroupMessages newMessage = GetNewPublicMessage();
            string expectedText = newMessage.Text;

            _mockDataAccess.Setup(p => p.AddAsync(It.IsAny<PublicGroupMessages>()))
                .ReturnsAsync(true);

            PublicMessagesService _service = GetNewService();

            var result = await _service.AddAsync(NewPublicMessageToDto(newMessage));

            Assert.Multiple(() =>
            {
                Assert.True(result.Success);
                Assert.Equal(expectedText, result.Data?.Text);
            });
        }

        [Fact]
        public async Task ModifyAsync_IsSuccess()
        {
            PublicGroupMessages messageToModify = GetExistingPublicMessage();
            string expectedText = messageToModify.Text;

            _mockDataAccess.Setup(p => p.ModifyAsync(It.IsAny<PublicGroupMessages>()))
                .ReturnsAsync(true);
            _mockDataAccess.Setup(p => p.Exists(messageToModify.PublicMessageId))
                .ReturnsAsync(true);
            _mockDataAccess.Setup(p => p.GetByMessageIdAsync(messageToModify.PublicMessageId))
                .ReturnsAsync(messageToModify);

            PublicMessagesService _service = GetNewService();

            var result = await _service.ModifyAsync(ModifiedPublicMessageToDto(messageToModify));

            Assert.Multiple(() =>
            {
                Assert.True(result.Success);
                Assert.Equal(expectedText, result.Data?.Text);
            });
        }

        [Fact]
        public async Task DeleteAsync_IsSuccess()
        {
            PublicGroupMessages messageToDelete = GetExistingPublicMessage();

            _mockDataAccess.Setup(p => p.DeleteAsync(It.IsAny<PublicGroupMessages>()))
                .ReturnsAsync(true);
            _mockDataAccess.Setup(p => p.Exists(messageToDelete.PublicMessageId))
                .ReturnsAsync(true);
            _mockDataAccess.Setup(p => p.GetByMessageIdAsync(messageToDelete.PublicMessageId))
                .ReturnsAsync(messageToDelete);
            _mockDataAccess.Setup(p => p.DeleteMessagesByResponseMessageIdAsync(messageToDelete.PublicMessageId))
                .ReturnsAsync(true);

            PublicMessagesService _service = GetNewService();

            var result = await _service.DeleteAsync(messageToDelete.PublicMessageId);

            Assert.True(result.Success);
            Assert.Equal("ok", result.Message);
        }

        #region PRIVATE METHODS

        private PublicMessagesService GetNewService()
        {
            return new(_mockDataAccess.Object, _mockSerilogger.Object);
        }

        private List<PublicGroupMessageDto> GetListPublicMessagesDto()
        {
            List<PublicGroupMessageDto> messageList = new()
            {
                new()
                {
                    PublicMessageId = Guid.Parse("e8ee70b6-678a-4b86-934e-da7f404a33a3"),
                    UserId          = "e1b9cf9a-ff86-4607-8765-9e47a305062a",
                    UserName        = "TestUser1",
                    ChatGroupId     = 1,
                    ChatGroupName   = "Test Chat Group 1",
                    Text            = "Sample message",
                    MessageDateTime = new DateTime(2023, 6, 15)
                },
                new()
                {
                    PublicMessageId = Guid.Parse("added9bc-7c2a-4673-995d-92f4c4432fdc"),
                    UserId          = "e1b9cf9a-ff86-4607-8765-9e47a305062a",
                    UserName        = "TestUser1",
                    ChatGroupId     = 1,
                    ChatGroupName   = "Test Chat Group 1",
                    Text            = "Sample message",
                    MessageDateTime = new DateTime(2023, 6, 15)
                },
                new()
                {
                    PublicMessageId = Guid.Parse("c57b308b-ca1a-4b85-919a-b147db30fde0"),
                    UserId          = "4eb0c266-894a-4c09-a6e2-4a0fb72e9c1c",
                    UserName        = "TestUser2",
                    ChatGroupId     = 1,
                    ChatGroupName   = "Test Chat Group 1",
                    Text            = "Sample message",
                    MessageDateTime = new DateTime(2023, 6, 15)
                },
                new()
                {
                    PublicMessageId = Guid.Parse("512fce5e-865a-4e4d-b6fd-2a57fb86149e"),
                    UserId          = "feac8ce0-5a21-4b89-9e23-beee9df517bb",
                    UserName        = "TestUser3",
                    ChatGroupId     = 2,
                    ChatGroupName   = "Test Chat Group 2",
                    Text            = "Sample message",
                    MessageDateTime = new DateTime(2023, 6, 15)
                },
                new()
                {
                    PublicMessageId = Guid.Parse("3eea1c79-61fb-41e0-852b-ab790835c827"),
                    UserId          = "8bc5d23a-9c70-4ef2-b285-814e993ad471",
                    UserName        = "TestUser4",
                    ChatGroupId     = 2,
                    ChatGroupName   = "Test Chat Group 2",
                    Text            = "Sample message",
                    MessageDateTime = new DateTime(2023, 6, 15)
                }
            };
            return messageList;
        }

        private PublicGroupMessages GetNewPublicMessage()
        {
            return new()
            {
                UserId          = "4eb0c266-894a-4c09-a6e2-4a0fb72e9c1c",
                ChatGroupId     = 1,
                Text            = "New Message.",
                MessageDateTime = DateTime.Now
            };
        }

        private PublicGroupMessages GetExistingPublicMessage()
        {
            return new()
            {
                PublicMessageId = Guid.Parse("e8ee70b6-678a-4b86-934e-da7f404a33a3"),
                UserId          = "e1b9cf9a-ff86-4607-8765-9e47a305062a",
                ChatGroupId     = 1,
                Text            = "Updated message",
                MessageDateTime = new DateTime(2023, 6, 15)
            };
        }

        private PublicGroupMessageDto NewPublicMessageToDto(PublicGroupMessages message)
        {
            return new()
            {
                UserId          = message.UserId,
                ChatGroupId     = message.ChatGroupId,
                Text            = message.Text,
                MessageDateTime = message.MessageDateTime,
                ReplyMessageId  = message.ReplyMessageId,
                PictureLink     = message.PictureLink
            };
        }

        private ModifyPublicGroupMessageDto ModifiedPublicMessageToDto(PublicGroupMessages message)
        {
            return new()
            {
                PublicMessageId = message.PublicMessageId,
                Text            = message.Text,
                ReplyMessageId  = message.ReplyMessageId,
                PictureLink     = message.PictureLink
            };
        }

        #endregion
    }
}
