using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SignalRBlazorGroupsMessages.API.Controllers;
using SignalRBlazorGroupsMessages.API.Helpers;
using SignalRBlazorGroupsMessages.API.Models;
using SignalRBlazorGroupsMessages.API.Models.Dtos;
using SignalRBlazorGroupsMessages.API.Services;

namespace SignalRBlazorUnitTests.SignalRBlazorGroupMessage.API.UnitTests.PrivateGroupMessagesTests
{
    public class PrivateGroupMessagesController_UnitTests
    {
        private readonly Mock<IPrivateGroupMessagesService> _mockService;
        private readonly Mock<ISerilogger> _mockSerilogger;
        private readonly Mock<IUserProvider> _mockUserProvider;

        public PrivateGroupMessagesController_UnitTests()
        {
            _mockService = new Mock<IPrivateGroupMessagesService>();
            _mockSerilogger = new Mock<ISerilogger>();
            _mockUserProvider = new Mock<IUserProvider>();
        }

        [Fact]
        public async Task GetListByGroupIdAsync_IsSuccess()
        {
            ApiResponse<List<PrivateGroupMessageDto>> apiResponse = new();
            int testGroupId = 1;
            List<PrivateGroupMessageDto> dtoList = GetDtoList()
                .Where(p => p.ChatGroupId == testGroupId)
                .ToList();

            _mockService.Setup(p => p.GetDtoListByGroupIdAsync(testGroupId, 0))
                .ReturnsAsync(ReturnApiResponse.Success(apiResponse, dtoList));

            PrivateGroupMessagesController _controller = GetNewController();

            var actionResult = await _controller.GetListByGroupIdAsync(testGroupId, 0);
            var objectResult = actionResult.Result as OkObjectResult;
            var result = (ApiResponse<List<PrivateGroupMessageDto>>)objectResult!.Value!;

            Assert.Multiple(() =>
            {
                Assert.True(result.Success);
                Assert.Equal(dtoList.Count, result!.Data!.Count);
            });
        }

        [Fact]
        public async Task GetListByUserIdAsync_IsSuccess()
        {
            ApiResponse<List<PrivateGroupMessageDto>> apiResponse = new();
            string testUserId = GetDtoList().First().UserId;
            List<PrivateGroupMessageDto> dtoList = GetDtoList()
                .Where(u => u.UserId == testUserId)
                .ToList();


            _mockService.Setup(p => p.GetDtoListByUserIdAsync(testUserId, 0))
                .ReturnsAsync(ReturnApiResponse.Success(apiResponse, dtoList));
            

            PrivateGroupMessagesController _controller = GetNewController();
            _mockUserProvider.Setup(x => x.GetUserIdClaim(_controller.ControllerContext.HttpContext))
                .Returns(testUserId);

            var actionResult = await _controller.GetListByUserIdAsync(testUserId, 0);
            var objectResult = actionResult.Result as OkObjectResult;
            var result = (ApiResponse<List<PrivateGroupMessageDto>>)objectResult!.Value!;

            Assert.Multiple(() =>
            {
                Assert.True(result.Success);
                Assert.Equal(dtoList.Count, result!.Data!.Count);
            });
        }

        #region PRIVATE METHODS

        private PrivateGroupMessagesController GetNewController()
        {
            return new(_mockService.Object, _mockSerilogger.Object, _mockUserProvider.Object);
        }

        private List<PrivateGroupMessageDto> GetDtoList()
        {
            return new()
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
        }

        #endregion
    }
}
