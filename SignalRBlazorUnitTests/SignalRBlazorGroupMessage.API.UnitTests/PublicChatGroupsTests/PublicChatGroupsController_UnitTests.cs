using Microsoft.AspNetCore.Mvc;
using Moq;
using SignalRBlazorGroupsMessages.API.Controllers;
using SignalRBlazorGroupsMessages.API.Helpers;
using SignalRBlazorGroupsMessages.API.Models;
using SignalRBlazorGroupsMessages.API.Models.Dtos;
using SignalRBlazorGroupsMessages.API.Services;

namespace SignalRBlazorUnitTests.SignalRBlazorGroupMessage.API.UnitTests.PublicChatGroups
{
	public class PublicChatGroupsController_UnitTests
    {
        private readonly Mock<IPublicChatGroupsService> _mockService;
        private readonly Mock<ISerilogger> _mockSerilogger;
        private readonly Mock<IUserProvider> _mockUserProvider;

        public PublicChatGroupsController_UnitTests()
        {
            _mockService = new Mock<IPublicChatGroupsService>();
            _mockSerilogger = new Mock<ISerilogger>();
            _mockUserProvider = new Mock<IUserProvider>();
        }

        [Fact]
        public async Task GetListByGroupIdAsync_ReturnsCorrectResult()
        {
            ApiResponse<List<PublicChatGroupsDto>> apiResponse = new();
            List<PublicChatGroupsDto> dtoList = GetDtoList();

            _mockService.Setup(p => p.GetListAsync())
                .ReturnsAsync(ReturnApiResponse.Success(apiResponse, dtoList));

            PublicChatGroupsController _controller = GetTestController();

            var actionResult = await _controller.GetPublicChatGroupsAsync();
            var objectResult = actionResult.Result as OkObjectResult;
            var result = (ApiResponse<List<PublicChatGroupsDto>>)objectResult!.Value!;

            Assert.Multiple(() =>
            {
                Assert.True(result.Success == true);
                Assert.Equal(dtoList.Count, result!.Data!.Count);
            });
        }

        [Fact]
        public async Task GetByIdAsync_IsSuccess()
        {
            ApiResponse<PublicChatGroupsDto> apiResponse = new();
            PublicChatGroupsDto dto = GetDtoList().First();
            int id = dto.ChatGroupId;

            PublicChatGroupsController _controller = GetTestController();

            _mockService.Setup(p => p.GetByIdAsync(id))
                .ReturnsAsync(ReturnApiResponse.Success(apiResponse, dto));
            _mockUserProvider.Setup(ip => ip.GetUserIP(_controller.HttpContext))
                .Returns("127.0.0.1");

            var actionResult = await _controller.GetByIdAsync(id);
            var objectResult = actionResult.Result as OkObjectResult;
            var result = (ApiResponse<PublicChatGroupsDto>)objectResult!.Value!;

            Assert.Multiple(() =>
            {
                Assert.True(result.Success);
                Assert.NotNull(result.Data);
                Assert.Equal(id, result.Data.ChatGroupId);
            });
        }

        [Fact]
        public async Task AddAsync_ReturnsCorrectResult()
        {
            ApiResponse<PublicChatGroupsDto> apiResponse = new();
            CreatePublicChatGroupDto createDto = GetCreateDto();

            PublicChatGroupsController _controller = GetTestController();

            _mockService.Setup(p => p.AddAsync(createDto))
                .ReturnsAsync(ReturnApiResponse.Success(apiResponse, CreatedDto()));
            _mockUserProvider.Setup(ip => ip.GetUserIdClaim(_controller.HttpContext))
                .Returns(createDto.GroupOwnerUserId);
            _mockUserProvider.Setup(ip => ip.GetUserIP(_controller.HttpContext))
                .Returns("127.0.0.1");

            var actionResult = await _controller.AddAsync(createDto);
            var objectResult = actionResult.Result as OkObjectResult;
            var result = (ApiResponse<PublicChatGroupsDto>)objectResult!.Value!;

            Assert.Multiple(() =>
            {
                Assert.True(result.Success);
                Assert.Equal(createDto.ChatGroupName, result.Data?.ChatGroupName);
            });
        }

        [Fact]
        public async Task ModifyAsync_ReturnscorrectResult()
        {
            ApiResponse<PublicChatGroupsDto> apiResponse = new();
            ModifyPublicChatGroupDto modifyDto = GetModifiedDto();
            PublicChatGroupsDto modifiedDto = GetDtoList().First();
            string jwtUserId = modifiedDto.GroupOwnerUserId;
            modifiedDto.ChatGroupName = modifyDto.ChatGroupName;

            PublicChatGroupsController _controller = GetTestController();

            _mockService.Setup(p => p.ModifyAsync(modifyDto, jwtUserId))
                .ReturnsAsync(ReturnApiResponse.Success(apiResponse, modifiedDto));
            _mockUserProvider.Setup(ip => ip.GetUserIdClaim(_controller.HttpContext))
                .Returns(jwtUserId);
            _mockUserProvider.Setup(ip => ip.GetUserIP(_controller.HttpContext))
                .Returns("127.0.0.1");


            var actionResult = await _controller.ModifyAsync(modifyDto);
            var objectResult = actionResult.Result as OkObjectResult;
            var result = (ApiResponse<PublicChatGroupsDto>)objectResult!.Value!;

            Assert.Multiple(() =>
            {
                Assert.True(result.Success);
                Assert.Equal(modifyDto.ChatGroupName, modifiedDto.ChatGroupName);
            });
        }

        [Fact]
        public async Task DeleteAsync_ReturnsCorrectResult()
        {
            ApiResponse<PublicChatGroupsDto> apiResponse = new();
            PublicChatGroupsDto dtoToDelete = GetDtoList().First();
            string jwtUserId = dtoToDelete.GroupOwnerUserId;

            PublicChatGroupsController _controller = GetTestController();

            _mockService.Setup(p => p.DeleteAsync(dtoToDelete.ChatGroupId, jwtUserId))
                .ReturnsAsync(ReturnApiResponse.Success(apiResponse, new()));
            _mockUserProvider.Setup(ip => ip.GetUserIdClaim(_controller.HttpContext))
                .Returns(jwtUserId);
            _mockUserProvider.Setup(ip => ip.GetUserIP(_controller.HttpContext))
                .Returns("127.0.0.1");

            var actionResult = await _controller.DeleteAsync(dtoToDelete.ChatGroupId);
            var objectResult = actionResult.Result as NoContentResult;

            Assert.True(objectResult!.StatusCode == 204);
        }

        #region PRIVATE METHODS

        private PublicChatGroupsController GetTestController()
        {
            return new(_mockService.Object, _mockSerilogger.Object, _mockUserProvider.Object);
        }

        private List<PublicChatGroupsDto> GetDtoList()
        {
            List<PublicChatGroupsDto> groupList = new()
            {
                new()
                {
                    ChatGroupId      = 1,
                    ChatGroupName    = "Test Group 1",
                    GroupCreated     = DateTime.Now,
                    GroupOwnerUserId = "e1b9cf9a-ff86-4607-8765-9e47a305062a",
                    UserName         = "Test Owner"
                },
                new()
                {
                    ChatGroupId      = 2,
                    ChatGroupName    = "Test Group 2",
                    GroupCreated     = DateTime.Now,
                    GroupOwnerUserId = "e1b9cf9a-ff86-4607-8765-9e47a305062a",
                    UserName         = "Test Owner"
                },
                new()
                {
                    ChatGroupId      = 3,
                    ChatGroupName    = "Test Group 3",
                    GroupCreated     = DateTime.Now,
                    GroupOwnerUserId = "e1b9cf9a-ff86-4607-8765-9e47a305062a",
                    UserName         = "Test Owner"
                },
                new()
                {
                    ChatGroupId      = 4,
                    ChatGroupName    = "Test Group 4",
                    GroupCreated     = DateTime.Now,
                    GroupOwnerUserId = "e1b9cf9a-ff86-4607-8765-9e47a305062a",
                    UserName         = "Test Owner"
                }
            };
            return groupList;
        }

        private CreatePublicChatGroupDto GetCreateDto()
        {
            return new()
            {
                ChatGroupName = "Test Group 5",
                GroupOwnerUserId = "e1b9cf9a-ff86-4607-8765-9e47a305062a"
            };
        }

        private PublicChatGroupsDto CreatedDto()
        {
            return new()
            {
                ChatGroupId = 5,
                ChatGroupName = "Test Group 5",
                GroupCreated = DateTime.Now,
                GroupOwnerUserId = "e1b9cf9a-ff86-4607-8765-9e47a305062a",
                UserName = "Test Owner"
            };
        }

        private ModifyPublicChatGroupDto GetModifiedDto()
        {
            return new()
            {
                ChatGroupId = 1,
                ChatGroupName = "Modified Test Group Name"
            };
        }

        #endregion
    }
}
