﻿using ChatApplicationModels;
using Moq;
using SignalRBlazorGroupsMessages.API.DataAccess;
using SignalRBlazorGroupsMessages.API.Helpers;
using SignalRBlazorGroupsMessages.API.Models.Dtos;
using SignalRBlazorGroupsMessages.API.Services;

namespace SignalRBlazorUnitTests.SignalRBlazorGroupMessage.API.UnitTests.PublicMessagesTests
{
    public class PublicGroupMessageService_UnitTests
    {
        private readonly Mock<IPublicGroupMessagesDataAccess> _mockDataAccess;
        private readonly Mock<ISerilogger> _mockSerilogger;

        public PublicGroupMessageService_UnitTests()
        {
            _mockDataAccess = new Mock<IPublicGroupMessagesDataAccess>();
            _mockSerilogger = new Mock<ISerilogger>();
        }

        [Fact]
        public async Task GetListByGroupIdAsync_ReturnsCorrectResults()
        {
            int expectedCount = GetListDto()
                .Where(p => p.ChatGroupId == 1)
                .ToList()
                .Count;

            _mockDataAccess.Setup(m => m.GetDtoListByGroupIdAsync(1, 0))
                .ReturnsAsync(GetListDto()
                    .Where(x => x.ChatGroupId == 1)
                    .ToList());

            PublicGroupMessagesService _service = GetNewService();

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
            int expectedCount = GetListDto()
                .Where(u => u.UserId == testUserId)
                .ToList()
                .Count;

            _mockDataAccess.Setup(m => m.GetDtoListByUserIdAsync(testUserId, 0))
                .ReturnsAsync(GetListDto()
                    .Where(u => u.UserId == testUserId)
                    .ToList());

            PublicGroupMessagesService _service = GetNewService();

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
            PublicGroupMessageDto dtoMessage = GetListDto()
                .Single(g => g.PublicMessageId == testMessageId);

            _mockDataAccess.Setup(p => p.GetDtoByMessageIdAsync(testMessageId))
                .ReturnsAsync(GetListDto()
                    .Single(m => m.PublicMessageId == testMessageId));

            PublicGroupMessagesService _service = GetNewService();

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
            CreatePublicGroupMessageDto createDto = GetCreateDto();
            string expectedText = createDto.Text;

            _mockDataAccess.Setup(g => g.GetDtoByMessageIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(CreatedDto());
            _mockDataAccess.Setup(p => p.AddAsync(It.IsAny<PublicGroupMessages>()))
                .ReturnsAsync(true);

            PublicGroupMessagesService _service = GetNewService();

            var result = await _service.AddAsync(createDto);

            Assert.Multiple(() =>
            {
                Assert.True(result.Success);
                Assert.Equal(expectedText, result.Data?.Text);
            });
        }

        [Fact]
        public async Task ModifyAsync_IsSuccess()
        {
            PublicGroupMessageDto dtoMessage = GetListDto().First();
            string jwtUserId = dtoMessage.UserId;
            ModifyPublicGroupMessageDto messageToModify = GetModifyDto(dtoMessage);
            string expectedText = messageToModify.Text;

            _mockDataAccess.Setup(p => p.ModifyAsync(It.IsAny<PublicGroupMessages>()))
                .ReturnsAsync(true);
            _mockDataAccess.Setup(p => p.GetByMessageIdAsync(dtoMessage.PublicMessageId))
                .ReturnsAsync(DtoToModel(dtoMessage));
            // modify text here for return, only change is Text field
            dtoMessage.Text = expectedText;
            _mockDataAccess.Setup(g => g.GetDtoByMessageIdAsync(dtoMessage.PublicMessageId))
                .ReturnsAsync(dtoMessage);

            PublicGroupMessagesService _service = GetNewService();

            var result = await _service.ModifyAsync(messageToModify, jwtUserId);

            Assert.Multiple(() =>
            {
                Assert.True(result.Success);
                Assert.Equal(expectedText, result.Data?.Text);
            });
        }

        [Fact]
        public async Task DeleteAsync_IsSuccess()
        {
            PublicGroupMessageDto messageToDelete = GetListDto().First();
            string jwtUserId = messageToDelete.UserId;
            

            _mockDataAccess.Setup(p => p.DeleteAsync(It.IsAny<PublicGroupMessages>()))
                .ReturnsAsync(true);
            _mockDataAccess.Setup(p => p.Exists(messageToDelete.PublicMessageId))
                .ReturnsAsync(true);
            _mockDataAccess.Setup(p => p.GetByMessageIdAsync(messageToDelete.PublicMessageId))
                .ReturnsAsync(DtoToModel(messageToDelete));
            _mockDataAccess.Setup(p => p.DeleteMessagesByResponseMessageIdAsync(messageToDelete.PublicMessageId))
                .ReturnsAsync(true);

            PublicGroupMessagesService _service = GetNewService();

            var result = await _service.DeleteAsync(messageToDelete.PublicMessageId, jwtUserId);

            Assert.True(result.Success);
            Assert.Equal("ok", result.Message);
        }

        #region PRIVATE METHODS

        private PublicGroupMessagesService GetNewService()
        {
            return new(_mockDataAccess.Object, _mockSerilogger.Object);
        }

        private List<PublicGroupMessageDto> GetListDto()
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

        private CreatePublicGroupMessageDto GetCreateDto()
        {
            return new()
            {
                UserId      = "e1b9cf9a-ff86-4607-8765-9e47a305062a",
                ChatGroupId = 1,
                Text        = "New message"
            };
        }

        private PublicGroupMessageDto CreatedDto()
        {
            return new()
            {
                PublicMessageId = Guid.NewGuid(),
                UserId          = "e1b9cf9a-ff86-4607-8765-9e47a305062a",
                UserName        = "TestUser1",
                ChatGroupId     = 1,
                ChatGroupName   = "Test Chat Group 1",
                Text            = "New message",
                MessageDateTime = DateTime.Now
            };
        }

        private ModifyPublicGroupMessageDto GetModifyDto(PublicGroupMessageDto dto)
        {
            return new()
            {
                PublicMessageId = dto.PublicMessageId,
                Text            = "Modified Text",
                ReplyMessageId  = dto.ReplyMessageId,
                PictureLink     = dto.PictureLink
            };
        }

        private PublicGroupMessages DtoToModel(PublicGroupMessageDto dto)
        {
            return new()
            {
                PublicMessageId = dto.PublicMessageId,
                UserId          = dto.UserId,
                ChatGroupId     = dto.ChatGroupId,
                Text            = dto.Text,
                MessageDateTime = dto.MessageDateTime,
                ReplyMessageId  = dto.ReplyMessageId,
                PictureLink     = dto.PictureLink
            };
        }

        #endregion
    }
}
