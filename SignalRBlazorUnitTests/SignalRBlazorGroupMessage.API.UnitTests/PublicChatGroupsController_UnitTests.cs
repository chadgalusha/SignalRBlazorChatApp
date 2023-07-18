using Microsoft.AspNetCore.Mvc;
using Moq;
using SignalRBlazorGroupsMessages.API.Controllers;
using SignalRBlazorGroupsMessages.API.Helpers;
using SignalRBlazorGroupsMessages.API.Models;
using SignalRBlazorGroupsMessages.API.Models.Dtos;
using SignalRBlazorGroupsMessages.API.Services;
using System.Net;

namespace SignalRBlazorUnitTests.SignalRBlazorGroupMessage.API.UnitTests
{
    public class PublicChatGroupsController_UnitTests
    {
        private readonly Mock<IPublicChatGroupsService> _mockService;
        private readonly Mock<ISerilogger> _mockSerilogger;

        public PublicChatGroupsController_UnitTests()
        {
            _mockService = new Mock<IPublicChatGroupsService>();
            _mockSerilogger = new Mock<ISerilogger>();
        }

        [Fact]
        public async Task GetListByGroupIdAsync_ReturnsCorrectResult()
        {
            ApiResponse<List<PublicChatGroupsDto>> response = new();
            List<PublicChatGroupsDto> dtoList = GetDtoList();

            _mockService.Setup(p => p.GetListPublicChatGroupsAsync())
                .ReturnsAsync(ReturnApiResponse.Success(response, dtoList));

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
        public async Task GetByIdAsync_ReturnsCorrectresult()
        {
            ApiResponse<PublicChatGroupsDto> apiResponse = new();
            PublicChatGroupsDto dto = GetDtoList().First();
            int id = dto.ChatGroupId;

            _mockService.Setup(p => p.GetDtoByIdAsync(id))
                .ReturnsAsync(ReturnApiResponse.Success(apiResponse, dto));

            PublicChatGroupsController _controller = GetTestController();

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

            _mockService.Setup(p => p.AddAsync(createDto))
                .ReturnsAsync(ReturnApiResponse.Success(apiResponse, CreatedDto()));

            PublicChatGroupsController _controller = GetTestController();

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
            modifiedDto.ChatGroupName = modifyDto.ChatGroupName;


            _mockService.Setup(p => p.ModifyAsync(modifyDto))
                .ReturnsAsync(ReturnApiResponse.Success(apiResponse, modifiedDto));

            PublicChatGroupsController _controller = GetTestController();

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
            PublicChatGroupsDto dtoToDelete = GetDtoList()
                .Single(p => p.ChatGroupId == 1);
            int idToDelete = 1;

            _mockService.Setup(p => p.DeleteAsync(idToDelete))
                .ReturnsAsync(ReturnApiResponse.Success(apiResponse, dtoToDelete));

            PublicChatGroupsController _controller = GetTestController();

            var actionResult = await _controller.DeleteAsync(idToDelete);
            var objectResult = actionResult.Result as NoContentResult;

            Assert.True(objectResult.StatusCode == (int)HttpStatusCode.NoContent);
        }

        #region PRIVATE METHODS

        private PublicChatGroupsController GetTestController()
        {
            return new(_mockService.Object, _mockSerilogger.Object);
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
                    GroupOwnerUserId = Guid.Parse("e1b9cf9a-ff86-4607-8765-9e47a305062a"),
                    UserName         = "Test Owner"
                },
                new()
                {
                    ChatGroupId      = 2,
                    ChatGroupName    = "Test Group 2",
                    GroupCreated     = DateTime.Now,
                    GroupOwnerUserId = Guid.Parse("e1b9cf9a-ff86-4607-8765-9e47a305062a"),
                    UserName         = "Test Owner"
                },
                new()
                {
                    ChatGroupId      = 3,
                    ChatGroupName    = "Test Group 3",
                    GroupCreated     = DateTime.Now,
                    GroupOwnerUserId = Guid.Parse("e1b9cf9a-ff86-4607-8765-9e47a305062a"),
                    UserName         = "Test Owner"
                },
                new()
                {
                    ChatGroupId      = 4,
                    ChatGroupName    = "Test Group 4",
                    GroupCreated     = DateTime.Now,
                    GroupOwnerUserId = Guid.Parse("e1b9cf9a-ff86-4607-8765-9e47a305062a"),
                    UserName         = "Test Owner"
                }
            };
            return groupList;
        }

        private CreatePublicChatGroupDto GetCreateDto()
        {
            return new()
            {
                ChatGroupName    = "Test Group 5",
                GroupOwnerUserId = Guid.Parse("e1b9cf9a-ff86-4607-8765-9e47a305062a")
            };
        }

        private PublicChatGroupsDto CreatedDto()
        {
            return new()
            {
                ChatGroupId      = 5,
                ChatGroupName    = "Test Group 5",
                GroupCreated     = DateTime.Now,
                GroupOwnerUserId = Guid.Parse("e1b9cf9a-ff86-4607-8765-9e47a305062a"),
                UserName         = "Test Owner"
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
