using ChatApplicationModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SignalRBlazorGroupsMessages.API.Controllers;
using SignalRBlazorGroupsMessages.API.Helpers;
using SignalRBlazorGroupsMessages.API.Models;
using SignalRBlazorGroupsMessages.API.Models.Dtos;
using SignalRBlazorGroupsMessages.API.Services;
using System.Data.Entity.Core.Objects;
using Xunit.Sdk;

namespace SignalRBlazorUnitTests.SignalRBlazorGroupMessage.API.UnitTests.PublicMessagesTests
{
    public class PublicMessagesController_UnitTests
    {
        private readonly Mock<IPublicMessagesService> _mockService;
        private readonly Mock<ISerilogger> _mockSerilogger;

        public PublicMessagesController_UnitTests()
        {
            _mockService = new Mock<IPublicMessagesService>();
            _mockSerilogger = new Mock<ISerilogger>();
        }

        [Fact]
        public async Task GetListByGroupIdAsync_ReturnsSuccess()
        {
            ApiResponse<List<PublicGroupMessageDto>> apiResponse = new();
            int testGroupId = 1;
            List<PublicGroupMessageDto> dtoList = GetDtoList()
                .Where(c => c.ChatGroupId == testGroupId)
                .ToList();


            _mockService.Setup(p => p.GetListByGroupIdAsync(testGroupId, 0))
                .ReturnsAsync(ReturnApiResponse.Success(apiResponse, dtoList));

            PublicMessagesController _controller = GetNewController();

            var actionResult = await _controller.GetListByGroupIdAsync(testGroupId, 0);
            var objectResult = actionResult.Result as OkObjectResult;
            var result = (ApiResponse<List<PublicGroupMessageDto>>)objectResult!.Value!;

            Assert.Multiple(() =>
            {
                Assert.True(result.Success);
                Assert.Equal(dtoList.Count, result!.Data!.Count);
            });
        }

        [Fact]
        public async Task GetListByUserIdAsync_ReturnsSuccess()
        {
            ApiResponse<List<PublicGroupMessageDto>> apiResponse = new();
            string testUserId = "e1b9cf9a-ff86-4607-8765-9e47a305062a";
            List<PublicGroupMessageDto> dtoList = GetDtoList()
                .Where(u => u.UserId == testUserId)
                .ToList();

            _mockService.Setup(p => p.GetListByUserIdAsync(testUserId, 0))
                .ReturnsAsync(ReturnApiResponse.Success(apiResponse, dtoList));

            PublicMessagesController _controller = GetNewController();

            var actionResult = await _controller.GetListByUserIdAsync(testUserId, 0);
            var objectResult = actionResult.Result as OkObjectResult;
            var result = (ApiResponse<List<PublicGroupMessageDto>>)objectResult!.Value!;

            Assert.Multiple(() =>
            {
                Assert.True(result.Success);
                Assert.Equal(dtoList.Count, result!.Data!.Count);
            });
        }

        [Fact]
        public async Task GetByMessageIdAsync_ReturnsSuccess()
        {
            ApiResponse<PublicGroupMessageDto> apiResponse = new();
            Guid testMessageId = Guid.Parse("e8ee70b6-678a-4b86-934e-da7f404a33a3");
            PublicGroupMessageDto dto = GetDtoList()
                .Single(m => m.PublicMessageId == testMessageId);

            _mockService.Setup(p => p.GetByMessageIdAsync(testMessageId))
                .ReturnsAsync(ReturnApiResponse.Success(apiResponse, dto));

            PublicMessagesController _controller = GetNewController();

            var actionResult = await _controller.GetByMessageIdAsync(testMessageId);
            var objectResult = actionResult.Result as OkObjectResult;
            var result = (ApiResponse<PublicGroupMessageDto>)objectResult!.Value!;

            Assert.Multiple(() =>
            {
                Assert.True(result.Success);
                Assert.Equal(dto.ChatGroupName, result.Data!.ChatGroupName);
            });
        }

        [Fact]
        public async Task AddAsync_ReturnsSuccess()
        {
            ApiResponse<PublicGroupMessageDto> apiResponse = new();
            PublicGroupMessageDto newDto = GetNewDto();

            _mockService.Setup(p => p.AddAsync(newDto))
                .ReturnsAsync(ReturnApiResponse.Success(apiResponse, newDto));

            PublicMessagesController _controller = GetNewController();

            var actionResult = await _controller.AddAsync(newDto);
            var objectResult = actionResult.Result as OkObjectResult;
            var result = (ApiResponse<PublicGroupMessageDto>)objectResult!.Value!;

            Assert.Multiple(() =>
            {
                Assert.True(result.Success);
                Assert.True(result.Message == "ok");
            });
        }

        [Fact]
        public async Task ModifyAsync_ReturnsSuccess()
        {
            ApiResponse<PublicGroupMessageDto> apiResponse = new();
            PublicGroupMessageDto dtoToModify = GetDtoList().First();
            string originalText = dtoToModify.Text;
            string modifiedText = "text is modified";
            dtoToModify.Text = modifiedText;
            ModifyPublicGroupMessageDto modifiedDto = DtoToModifiedDto(dtoToModify);

            _mockService.Setup(p => p.ModifyAsync(modifiedDto))
                .ReturnsAsync(ReturnApiResponse.Success(apiResponse, dtoToModify));

            PublicMessagesController _controller = GetNewController();

            var actionResult = await _controller.ModifyAsync(modifiedDto);
            var objectResult = actionResult.Result as OkObjectResult;
            var result = (ApiResponse<PublicGroupMessageDto>)objectResult!.Value!;

            Assert.Multiple(() =>
            {
                Assert.True(result.Success);
                Assert.True(result.Message == "ok");
                Assert.Equal(modifiedText, result.Data!.Text);
            });
        }

        [Fact]
        public async Task DeleteAsync_IsSuccess()
        {
            ApiResponse<PublicGroupMessageDto> apiResponse = new();
            PublicGroupMessageDto dtoToDelete = GetDtoList().First();

            _mockService.Setup(p => p.DeleteAsync(dtoToDelete.PublicMessageId))
                .ReturnsAsync(ReturnApiResponse.Success(apiResponse, dtoToDelete));

            PublicMessagesController _controller = GetNewController();

            var actionResult = await _controller.DeleteAsync(dtoToDelete.PublicMessageId);
            var objectResult = actionResult.Result as NoContentResult;

            Assert.Multiple(() =>
            {
                Assert.True(objectResult!.StatusCode == 204);
            });
        }

        [Fact]
        public async Task DeleteAsync_Returns404()
        {
            ApiResponse<PublicGroupMessageDto> apiResponse = new();
            PublicGroupMessageDto dtoToDelete = GetNewDto();

            _mockService.Setup(p => p.DeleteAsync(dtoToDelete.PublicMessageId))
                .ReturnsAsync(ReturnApiResponse.Failure(apiResponse, "Message Id not found."));

            PublicMessagesController _controller = GetNewController();

            var actionResult = await _controller.DeleteAsync(dtoToDelete.PublicMessageId);
            var objectResult = actionResult.Result as NotFoundObjectResult;
            var result = (ApiResponse<PublicGroupMessageDto>)objectResult!.Value!;

            Assert.Multiple(() =>
            {
                Assert.False(result.Success);
                Assert.True(objectResult.StatusCode == 404);
            });
        }

        [Fact]
        public async Task DeleteAsync_Returns500()
        {
            ApiResponse<PublicGroupMessageDto> apiResponse = new();
            PublicGroupMessageDto dtoToDelete = new();

            _mockService.Setup(p => p.DeleteAsync(dtoToDelete.PublicMessageId))
                .ReturnsAsync(ReturnApiResponse.Failure(apiResponse, "Error deleting message."));

            PublicMessagesController _controller = GetNewController();

            var actionResult = await _controller.DeleteAsync(dtoToDelete.PublicMessageId);
            var objectResult = actionResult.Result as Microsoft.AspNetCore.Mvc.ObjectResult;
            var result = (ApiResponse<PublicGroupMessageDto>)objectResult!.Value!;

            Assert.Multiple(() =>
            {
                Assert.True(objectResult.StatusCode == 500);
                Assert.False(result.Success);
            });
        }

        #region PRIVATE METHODS

        private PublicMessagesController GetNewController()
        {
            return new(_mockService.Object, _mockSerilogger.Object);
        }

        private List<PublicGroupMessageDto> GetDtoList()
        {
            return new()
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
        }

        private PublicGroupMessageDto GetNewDto()
        {
            return new()
            {
                PublicMessageId = Guid.Parse("6437aba6-502e-4a69-b675-37e0c80015b2"),
                UserId          = "8bc5d23a-9c70-4ef2-b285-814e993ad471",
                UserName        = "TestUser4",
                ChatGroupId     = 1,
                ChatGroupName   = "Test Chat Group 1",
                Text            = "Sample message",
                MessageDateTime = DateTime.Now
            };
        }

        private ModifyPublicGroupMessageDto DtoToModifiedDto(PublicGroupMessageDto dto)
        {
            return new()
            {
                PublicMessageId = dto.PublicMessageId,
                Text            = dto.Text,
                ReplyMessageId  = dto.ReplyMessageId,
                PictureLink     = dto.PictureLink
            };
        }

        #endregion
    }
}
