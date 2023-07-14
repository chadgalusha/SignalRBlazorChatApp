using ChatApplicationModels;
using Moq;
using SignalRBlazorGroupsMessages.API.DataAccess;
using SignalRBlazorGroupsMessages.API.Helpers;
using SignalRBlazorGroupsMessages.API.Models;
using SignalRBlazorGroupsMessages.API.Services;

namespace SignalRBlazorUnitTests.SignalRBlazorGroupMessage.API.UnitTests
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
            int expectedCount = GetListPublicMessagesView()
                .Where(p => p.ChatGroupId == 1)
                .ToList()
                .Count;

            _mockDataAccess.Setup(m => m.GetViewListByGroupIdAsync(1, 0))
                .ReturnsAsync(GetListPublicMessagesView()
                    .Where(x => x.ChatGroupId == 1)
                    .ToList());

            PublicMessagesService _service = new(_mockDataAccess.Object, _mockSerilogger.Object);

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
            Guid testUserId = Guid.Parse("e8ee70b6-678a-4b86-934e-da7f404a33a3");
            int expectedCount = GetListPublicMessagesView()
                .Where(u => u.UserId == testUserId)
                .ToList()
                .Count;

            _mockDataAccess.Setup(m => m.GetViewListByUserIdAsync(testUserId, 0))
                .ReturnsAsync(GetListPublicMessagesView()
                    .Where(u => u.UserId == testUserId)
                    .ToList());

            PublicMessagesService _service = new(_mockDataAccess.Object, _mockSerilogger.Object);
                
            var result1 = await _service.GetViewListByUserIdAsync(testUserId, 0);
            var result2 = await _service.GetViewListByUserIdAsync(Guid.Parse("5e34cdf3-7ecc-46df-87ea-4bb1839af3d6"), 0);

            Assert.Multiple(() =>
            {
                Assert.Equal(expectedCount, result1.Data?.Count);
                Assert.True(result1.Success);

                Assert.False(result2.Success);
                Assert.Null(result2.Data);
            });
        }

        [Fact]
        public async Task GetByMessageIdAsync_ReturnsMessage()
        {
            Guid testMessageId = Guid.Parse("e8ee70b6-678a-4b86-934e-da7f404a33a3");
            PublicGroupMessagesView viewMessage = GetListPublicMessagesView()
                .Single(g => g.PublicMessageId == testMessageId);
            PublicGroupMessageDto expectedDtoMessage = ViewToDto(viewMessage);

            _mockDataAccess.Setup(p => p.GetViewByMessageIdAsync(testMessageId))
                .ReturnsAsync(GetListPublicMessagesView()
                    .Single(m => m.PublicMessageId == testMessageId));

            PublicMessagesService _service = new(_mockDataAccess.Object, _mockSerilogger.Object);

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

            PublicMessagesService _service = new(_mockDataAccess.Object, _mockSerilogger.Object);

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

            PublicMessagesService _service = new(_mockDataAccess.Object, _mockSerilogger.Object);

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

            PublicMessagesService _service = new(_mockDataAccess.Object, _mockSerilogger.Object);

            var result = await _service.DeleteAsync(ModifiedPublicMessageToDto(messageToDelete));

            Assert.True(result.Success);
            Assert.Equal("ok", result.Message);
        }

        #region PRIVATE METHODS

        private List<PublicGroupMessagesView> GetListPublicMessagesView()
        {
            List<PublicGroupMessagesView> messageList = new()
            {
                new()
                {
                    PublicMessageId = Guid.Parse("e8ee70b6-678a-4b86-934e-da7f404a33a3"),
                    UserId          = Guid.Parse("e1b9cf9a-ff86-4607-8765-9e47a305062a"),
                    UserName        = "TestUser1",
                    ChatGroupId     = 1,
                    ChatGroupName   = "Test Chat Group 1",
                    Text            = "Sample message",
                    MessageDateTime = new DateTime(2023, 6, 15)
                },
                new()
                {
                    PublicMessageId = Guid.Parse("added9bc-7c2a-4673-995d-92f4c4432fdc"),
                    UserId          = Guid.Parse("e1b9cf9a-ff86-4607-8765-9e47a305062a"),
                    UserName        = "TestUser1",
                    ChatGroupId     = 1,
                    ChatGroupName   = "Test Chat Group 1",
                    Text            = "Sample message",
                    MessageDateTime = new DateTime(2023, 6, 15)
                },
                new()
                {
                    PublicMessageId = Guid.Parse("c57b308b-ca1a-4b85-919a-b147db30fde0"),
                    UserId          = Guid.Parse("4eb0c266-894a-4c09-a6e2-4a0fb72e9c1c"),
                    UserName        = "TestUser2",
                    ChatGroupId     = 1,
                    ChatGroupName   = "Test Chat Group 1",
                    Text            = "Sample message",
                    MessageDateTime = new DateTime(2023, 6, 15)
                },
                new()
                {
                    PublicMessageId = Guid.Parse("512fce5e-865a-4e4d-b6fd-2a57fb86149e"),
                    UserId          = Guid.Parse("feac8ce0-5a21-4b89-9e23-beee9df517bb"),
                    UserName        = "TestUser3",
                    ChatGroupId     = 2,
                    ChatGroupName   = "Test Chat Group 2",
                    Text            = "Sample message",
                    MessageDateTime = new DateTime(2023, 6, 15)
                },
                new()
                {
                    PublicMessageId = Guid.Parse("3eea1c79-61fb-41e0-852b-ab790835c827"),
                    UserId          = Guid.Parse("8bc5d23a-9c70-4ef2-b285-814e993ad471"),
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
                UserId          = Guid.Parse("4eb0c266-894a-4c09-a6e2-4a0fb72e9c1c"),
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
                UserId          = Guid.Parse("e1b9cf9a-ff86-4607-8765-9e47a305062a"),
                ChatGroupId     = 1,
                Text            = "Updated message",
                MessageDateTime = new DateTime(2023, 6, 15)
            };
        }

        private PublicGroupMessageDto NewPublicMessageToDto(PublicGroupMessages message)
        {
            return new()
            {
                UserId = message.UserId,
                ChatGroupId = message.ChatGroupId,
                Text = message.Text,
                MessageDateTime = message.MessageDateTime,
                ReplyMessageId = message.ReplyMessageId,
                PictureLink = message.PictureLink
            };
        }

        private PublicGroupMessageDto ModifiedPublicMessageToDto(PublicGroupMessages message)
        {
            return new()
            {
                PublicMessageId = message.PublicMessageId,
                UserId          = message.UserId,
                ChatGroupId     = message.ChatGroupId,
                Text            = message.Text,
                MessageDateTime = message.MessageDateTime,
                ReplyMessageId  = message.ReplyMessageId,
                PictureLink     = message.PictureLink
            };
        }

        private PublicGroupMessageDto ViewToDto(PublicGroupMessagesView publicMessagesView)
        {
            return new()
            {
                PublicMessageId = publicMessagesView.PublicMessageId,
                UserId = publicMessagesView.UserId,
                UserName = publicMessagesView.UserName,
                ChatGroupId = publicMessagesView.ChatGroupId,
                ChatGroupName = publicMessagesView.ChatGroupName,
                Text = publicMessagesView.Text,
                MessageDateTime = publicMessagesView.MessageDateTime,
                ReplyMessageId = publicMessagesView.ReplyMessageId,
                PictureLink = publicMessagesView.PictureLink
            };
        }

        #endregion
    }
}
