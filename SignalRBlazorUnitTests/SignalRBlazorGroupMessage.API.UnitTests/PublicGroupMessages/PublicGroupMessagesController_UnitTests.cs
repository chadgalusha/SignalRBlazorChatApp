using Microsoft.AspNetCore.Mvc;
using Moq;
using SignalRBlazorGroupsMessages.API.Controllers;
using SignalRBlazorGroupsMessages.API.Helpers;
using SignalRBlazorGroupsMessages.API.Models;
using SignalRBlazorGroupsMessages.API.Models.Dtos;
using SignalRBlazorGroupsMessages.API.Services;

namespace SignalRBlazorUnitTests.SignalRBlazorGroupMessage.API.UnitTests.PublicMessages
{
    public class PublicGroupMessagesController_UnitTests
    {
        private readonly Mock<IPublicGroupMessagesService> _mockService;
        private readonly Mock<ISerilogger> _mockSerilogger;
        private readonly Mock<IUserProvider> _mockUserProvider;

        public PublicGroupMessagesController_UnitTests()
        {
            _mockService = new Mock<IPublicGroupMessagesService>();
            _mockSerilogger = new Mock<ISerilogger>();
            _mockUserProvider = new Mock<IUserProvider>();
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

            PublicGroupMessagesController _controller = GetNewController();

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
        public async Task GetListByUserIdAsync_IsSuccess()
        {
            ApiResponse<List<PublicGroupMessageDto>> apiResponse = new();
            string testUserId = GetDtoList().First().UserId;
            List<PublicGroupMessageDto> dtoList = GetDtoList()
                .Where(u => u.UserId == testUserId)
                .ToList();

            _mockService.Setup(p => p.GetListByUserIdAsync(testUserId, 0))
                .ReturnsAsync(ReturnApiResponse.Success(apiResponse, dtoList));

            PublicGroupMessagesController _controller = GetNewController();

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
        public async Task GetByMessageIdAsync_IsSuccess()
        {
            ApiResponse<PublicGroupMessageDto> apiResponse = new();
            Guid testMessageId = GetDtoList().First().PublicMessageId;
            PublicGroupMessageDto dto = GetDtoList()
                .Single(m => m.PublicMessageId == testMessageId);

            _mockService.Setup(p => p.GetByMessageIdAsync(testMessageId))
                .ReturnsAsync(ReturnApiResponse.Success(apiResponse, dto));

            PublicGroupMessagesController _controller = GetNewController();

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
        public async Task AddAsync_IsSuccess()
        {
            ApiResponse<PublicGroupMessageDto> apiResponse = new();
            CreatePublicGroupMessageDto createDto = GetCreateDto();
            PublicGroupMessageDto newDto = CreatedDto();
            string jwtUserId = newDto.UserId;

            PublicGroupMessagesController _controller = GetNewController();

            _mockUserProvider.Setup(u => u.GetUserIdClaim(_controller.HttpContext))
                .Returns(jwtUserId);
            _mockUserProvider.Setup(ip => ip.GetUserIP(_controller.HttpContext))
                .Returns("127.0.0.1");
            _mockService.Setup(p => p.AddAsync(createDto))
                .ReturnsAsync(ReturnApiResponse.Success(apiResponse, newDto));

            var actionResult = await _controller.AddAsync(createDto);
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
            string jwtUserId = dtoToModify.UserId;
            string originalText = dtoToModify.Text;
            string modifiedText = "text is modified";
            dtoToModify.Text = modifiedText;
            ModifyPublicGroupMessageDto modifiedDto = DtoToModifiedDto(dtoToModify);

            PublicGroupMessagesController _controller = GetNewController();

            _mockUserProvider.Setup(u => u.GetUserIdClaim(_controller.HttpContext))
                .Returns(jwtUserId);
            _mockUserProvider.Setup(ip => ip.GetUserIP(_controller.HttpContext))
                .Returns("127.0.0.1");
            _mockService.Setup(p => p.ModifyAsync(modifiedDto, jwtUserId))
                .ReturnsAsync(ReturnApiResponse.Success(apiResponse, dtoToModify));

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
            string jwtUserId = dtoToDelete.UserId;

            PublicGroupMessagesController _controller = GetNewController();

            _mockUserProvider.Setup(u => u.GetUserIdClaim(_controller.HttpContext))
                .Returns(jwtUserId);
            _mockUserProvider.Setup(ip => ip.GetUserIP(_controller.HttpContext))
                .Returns("127.0.0.1");
            _mockService.Setup(p => p.DeleteAsync(dtoToDelete.PublicMessageId, jwtUserId))
                .ReturnsAsync(ReturnApiResponse.Success(apiResponse, new()));

            var actionResult = await _controller.DeleteAsync(dtoToDelete.PublicMessageId);
            var objectResult = actionResult.Result as NoContentResult;

            Assert.True(objectResult!.StatusCode == 204);
        }

        [Fact]
        public async Task DeleteAsync_Returns404()
        {
            ApiResponse<PublicGroupMessageDto> apiResponse = new();
            PublicGroupMessageDto dtoToDelete = GetDtoList().First();
            string jwtUserId = dtoToDelete.UserId;

            PublicGroupMessagesController _controller = GetNewController();

            _mockUserProvider.Setup(u => u.GetUserIdClaim(_controller.HttpContext))
                .Returns(jwtUserId);
            _mockUserProvider.Setup(ip => ip.GetUserIP(_controller.HttpContext))
                .Returns("127.0.0.1");
            _mockService.Setup(p => p.DeleteAsync(dtoToDelete.PublicMessageId, jwtUserId))
                .ReturnsAsync(ReturnApiResponse.Failure(apiResponse, "Message Id not found."));

            var actionResult = await _controller.DeleteAsync(dtoToDelete.PublicMessageId);
            var objectResult = actionResult.Result as ObjectResult;
            var result = (ApiResponse<PublicGroupMessageDto>)objectResult!.Value!;

            Assert.Multiple(() =>
            {
                Assert.False(result.Success);
                Assert.True(objectResult.StatusCode == 404);
            });
        }

        #region PRIVATE METHODS

        private PublicGroupMessagesController GetNewController()
        {
            return new(_mockService.Object, _mockSerilogger.Object, _mockUserProvider.Object);
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

        private CreatePublicGroupMessageDto GetCreateDto()
        {
            return new()
            {
                UserId          = "8bc5d23a-9c70-4ef2-b285-814e993ad471",
                ChatGroupId     = 1,
                Text            = "Sample message"
            };
        }

        private PublicGroupMessageDto CreatedDto()
        {
            return new()
            {
                PublicMessageId = Guid.Parse("6437aba6-502e-4a69-b675-37e0c80015b2"),
                UserId = "8bc5d23a-9c70-4ef2-b285-814e993ad471",
                UserName = "TestUser4",
                ChatGroupId = 1,
                ChatGroupName = "Test Chat Group 1",
                Text = "Sample message",
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
