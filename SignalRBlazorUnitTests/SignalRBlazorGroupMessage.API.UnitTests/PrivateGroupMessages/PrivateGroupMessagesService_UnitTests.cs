using Moq;
using SignalRBlazorGroupsMessages.API.DataAccess;
using SignalRBlazorGroupsMessages.API.Helpers;
using SignalRBlazorGroupsMessages.API.Models.Dtos;
using SignalRBlazorGroupsMessages.API.Services;

namespace SignalRBlazorUnitTests.SignalRBlazorGroupMessage.API.UnitTests.PrivateGroupMessages
{
	public class PrivateGroupMessagesService_UnitTests
    {
        private readonly Mock<IPrivateGroupMessagesDataAccess> _mockDataAccess;
        private readonly Mock<ISerilogger> _mockSerilogger;

        public PrivateGroupMessagesService_UnitTests()
        {
            _mockDataAccess = new Mock<IPrivateGroupMessagesDataAccess>();
            _mockSerilogger = new Mock<ISerilogger>();
        }

        [Fact]
        public async Task GetDtoListByGroupIdAsync_IsSuccess()
        {
            int expectedCount = GetListPrivateMessagesDto()
                .Where(c => c.ChatGroupId == 1)
                .ToList()
                .Count;

            _mockDataAccess.Setup(g => g.GroupIdExists(It.IsAny<int>()))
                .ReturnsAsync(true);

            _mockDataAccess.Setup(p => p.GetDtoListByGroupIdAsync(1, 0))
                .ReturnsAsync(GetListPrivateMessagesDto()
                    .Where(c => c.ChatGroupId == 1)
                    .ToList());

            PrivateGroupMessagesService _service = GetNewService();

            var result = await _service.GetDtoListByGroupIdAsync(1, 0);

            Assert.Multiple(() =>
            {
                Assert.Equal(expectedCount, result.Data?.Count);
                Assert.True(result.Success);
            });
        }

        [Fact]
        public async Task GetDtoListByUserIdAsync_IsSuccess()
        {
            string testUserId = GetListPrivateMessagesDto().First().UserId;
            int expectedCount = GetListPrivateMessagesDto()
                .Where(u => u.UserId == testUserId)
                .ToList()
                .Count;

            _mockDataAccess.Setup(p => p.GetDtoListByUserIdAsync(testUserId, 0))
                .ReturnsAsync(GetListPrivateMessagesDto()
                    .Where(u => u.UserId == testUserId)
                    .ToList());

            PrivateGroupMessagesService _service = GetNewService();

            var result = await _service.GetDtoListByUserIdAsync(testUserId, 0);

            Assert.Multiple(() =>
            {
                Assert.Equal(expectedCount, result.Data?.Count);
                Assert.True(result.Success);
            });
        }

        [Fact]
        public async Task GetDtoByMessageIdAsync_IsSuccess()
        {
            Guid testMessageId = GetListPrivateMessagesDto().First().PrivateMessageId;

            _mockDataAccess.Setup(p => p.GetDtoByMessageIdAsync(testMessageId))
                .ReturnsAsync(GetListPrivateMessagesDto()
                    .Single(m => m.PrivateMessageId == testMessageId));

            PrivateGroupMessagesService _service = GetNewService();

            var result = await _service.GetDtoByMessageIdAsync(testMessageId);

            Assert.Multiple(() =>
            {
                Assert.Equal(testMessageId, result.Data?.PrivateMessageId);
                Assert.True(result.Success);
            });
        }

        [Fact]
        public async Task AddAsync_IsSuccess()
        {
            CreatePrivateGroupMessageDto createDto = GetCreateDto();
            ChatApplicationModels.PrivateGroupMessages newMessage = GetNewModel(createDto);

            _mockDataAccess.Setup(p => p.AddAsync(It.IsAny<ChatApplicationModels.PrivateGroupMessages>()))
                .ReturnsAsync(true);

            PrivateGroupMessagesService _service = GetNewService();

            var result = await _service.AddAsync(createDto);

            Assert.True(result.Success);
        }

        [Fact]
        public async Task ModifyAsync_IsSucess()
        {
            ModifyPrivateGroupMessageDto dtoToModify = GetModifyDto();
            var returnDto = GetListPrivateMessagesDto().First();
            returnDto.Text = dtoToModify.Text;
            string testJwtUserId = returnDto.UserId;


            _mockDataAccess.Setup(p => p.GetByMessageIdAsync(dtoToModify.PrivateMessageId))
                .ReturnsAsync(DtoToModel(GetListPrivateMessagesDto().First()));
            _mockDataAccess.Setup(p => p.ModifyAsync(It.IsAny<ChatApplicationModels.PrivateGroupMessages>()))
                .ReturnsAsync(true);
            _mockDataAccess.Setup(p => p.GetDtoByMessageIdAsync(dtoToModify.PrivateMessageId))
                .ReturnsAsync(returnDto);

            PrivateGroupMessagesService _service = GetNewService();

            var result = await _service.ModifyAsync(dtoToModify, testJwtUserId);

            Assert.Multiple(() =>
            {
                Assert.True(result.Success);
                Assert.Equal(dtoToModify.Text, result.Data?.Text);
            });
        }

        [Fact]
        public async Task DeleteAsync_IsSuccess()
        {
            Guid messageId = GetListPrivateMessagesDto().First().PrivateMessageId;
            string testJwtUserId = GetListPrivateMessagesDto().First().UserId;

            _mockDataAccess.Setup(p => p.GetByMessageIdAsync(messageId))
                .ReturnsAsync(DtoToModel(
                    GetListPrivateMessagesDto()
                        .Single(m => m.PrivateMessageId == messageId)));
            _mockDataAccess.Setup(p => p.DeleteMessagesByReplyMessageIdAsync(messageId))
                .ReturnsAsync(true);
            _mockDataAccess.Setup(p => p.DeleteAsync(It.IsAny<ChatApplicationModels.PrivateGroupMessages>()))
                .ReturnsAsync(true);

            PrivateGroupMessagesService _service = GetNewService();

            var result = await _service.DeleteAsync(messageId, testJwtUserId);
            
            Assert.Multiple(() =>
            {
                Assert.True(result.Success);
                Assert.Equal(new Guid(), result.Data?.PrivateMessageId);
            });
        }

        [Fact]
        public async Task DeleteAllMessagesInGroupAsync_IsSuccess()
        {
            int groupId = 1;

            _mockDataAccess.Setup(p => p.DeleteAllMessagesInGroupAsync(groupId))
                .ReturnsAsync(true);

            PrivateGroupMessagesService _service = GetNewService();

            var result = await _service.DeleteAllMessagesInGroupAsync(groupId);

            Assert.True(result);
        }

        #region PRIVATE METHODS

        private PrivateGroupMessagesService GetNewService()
        {
            return new(_mockDataAccess.Object, _mockSerilogger.Object);
        }

        private List<PrivateGroupMessageDto> GetListPrivateMessagesDto()
        {
            List<PrivateGroupMessageDto> messageList = new()
            {
                new()
                {
                    PrivateMessageId = Guid.Parse("e8ee70b6-678a-4b86-934e-da7f404a33a3"),
                    UserId          = "e1b9cf9a-ff86-4607-8765-9e47a305062a",
                    UserName        = "TestUser1",
                    ChatGroupId     = 1,
                    ChatGroupName   = "Test Chat Group 1",
                    Text            = "Sample message",
                    MessageDateTime = new DateTime(2023, 6, 15)
                },
                new()
                {
                    PrivateMessageId = Guid.Parse("added9bc-7c2a-4673-995d-92f4c4432fdc"),
                    UserId          = "e1b9cf9a-ff86-4607-8765-9e47a305062a",
                    UserName        = "TestUser1",
                    ChatGroupId     = 1,
                    ChatGroupName   = "Test Chat Group 1",
                    Text            = "Sample message",
                    MessageDateTime = new DateTime(2023, 6, 15)
                },
                new()
                {
                    PrivateMessageId = Guid.Parse("c57b308b-ca1a-4b85-919a-b147db30fde0"),
                    UserId          = "4eb0c266-894a-4c09-a6e2-4a0fb72e9c1c",
                    UserName        = "TestUser2",
                    ChatGroupId     = 1,
                    ChatGroupName   = "Test Chat Group 1",
                    Text            = "Sample message",
                    MessageDateTime = new DateTime(2023, 6, 15)
                },
                new()
                {
                    PrivateMessageId = Guid.Parse("512fce5e-865a-4e4d-b6fd-2a57fb86149e"),
                    UserId          = "feac8ce0-5a21-4b89-9e23-beee9df517bb",
                    UserName        = "TestUser3",
                    ChatGroupId     = 2,
                    ChatGroupName   = "Test Chat Group 2",
                    Text            = "Sample message",
                    MessageDateTime = new DateTime(2023, 6, 15)
                },
                new()
                {
                    PrivateMessageId = Guid.Parse("3eea1c79-61fb-41e0-852b-ab790835c827"),
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

        private CreatePrivateGroupMessageDto GetCreateDto()
        {
            return new()
            {
                UserId      = "e1b9cf9a-ff86-4607-8765-9e47a305062a",
                ChatGroupId = 1,
                Text        = "New Message."
            };
        }

        private ChatApplicationModels.PrivateGroupMessages GetNewModel(CreatePrivateGroupMessageDto createDto)
        {
            return new()
            {
                PrivateMessageId = Guid.NewGuid(),
                UserId           = createDto.UserId,
                ChatGroupId      = createDto.ChatGroupId,
                Text             = createDto.Text,
                MessageDateTime  = DateTime.Now,
                ReplyMessageId   = createDto.ReplyMessageId,
                PictureLink      = createDto.PictureLink
            };
        }

        private ChatApplicationModels.PrivateGroupMessages DtoToModel(PrivateGroupMessageDto dto)
        {
            return new()
            {
                PrivateMessageId = dto.PrivateMessageId,
                UserId           = dto.UserId,
                ChatGroupId      = dto.ChatGroupId,
                Text             = dto.Text,
                MessageDateTime  = dto.MessageDateTime,
                ReplyMessageId   = dto.ReplyMessageId,
                PictureLink      = dto.PictureLink
            };
        }

        private ModifyPrivateGroupMessageDto GetModifyDto()
        {
            return new()
            {
                PrivateMessageId = Guid.Parse("e8ee70b6-678a-4b86-934e-da7f404a33a3"),
                Text             = "Modified Text."
            };
        }

        #endregion
    }
}
