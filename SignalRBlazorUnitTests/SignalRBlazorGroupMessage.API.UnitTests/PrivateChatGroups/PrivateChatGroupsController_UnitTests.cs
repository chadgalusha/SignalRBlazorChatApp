using ChatApplicationModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SignalRBlazorGroupsMessages.API.Controllers;
using SignalRBlazorGroupsMessages.API.Helpers;
using SignalRBlazorGroupsMessages.API.Models;
using SignalRBlazorGroupsMessages.API.Models.Dtos;
using SignalRBlazorGroupsMessages.API.Services;

namespace SignalRBlazorUnitTests.SignalRBlazorGroupMessage.API.UnitTests.PrivateChatGroups
{
    public class PrivateChatGroupsController_UnitTests : ControllerBase
    {
        private readonly Mock<IPrivateChatGroupsService> _mockService;
        private readonly Mock<IUserProvider> _mockUserProvider;
        private readonly Mock<ISerilogger> _mockSerilloger;
        private const string LocalHost = "127.0.0.1";

        public PrivateChatGroupsController_UnitTests()
        {
            _mockService = new Mock<IPrivateChatGroupsService>();
            _mockUserProvider = new Mock<IUserProvider>();
            _mockSerilloger = new Mock<ISerilogger>();
        }

        [Fact]
        public async Task GetDtoListByUserIdAsync_IsSuccess()
        {
            ApiResponse<List<PrivateChatGroupsDto>> apiResponse = new();
            string testUserId = Guid.NewGuid().ToString();
            List<PrivateChatGroupsDto> dtoList = new();

            var _controller = GetNewController();

            _mockService.Setup(p => p.GetDtoListByUserIdAsync(testUserId))
                .ReturnsAsync(ReturnApiResponse.Success(apiResponse, dtoList));
            _mockUserProvider.Setup(id => id.GetUserIdClaim(_controller.HttpContext))
                .Returns(testUserId);
            _mockUserProvider.Setup(ip => ip.GetUserIP(_controller.HttpContext))
                .Returns(LocalHost);

            var actionResult = await _controller.GetDtoListByUserIdAsync(testUserId);
            var objectResult = actionResult.Result as OkObjectResult;
            var result = (ApiResponse<List<PrivateChatGroupsDto>>)objectResult!.Value!;

            Assert.True(result.Success);
        }

        [Fact]
        public async Task GetDtoListByUserIdAsync_Returns403()
        {
            ApiResponse<List<PrivateChatGroupsDto>> apiResponse = new();
            string testUserId = Guid.NewGuid().ToString();

            var _controller = GetNewController();

            _mockUserProvider.Setup(u => u.GetUserIdClaim(_controller.HttpContext))
                .Returns(new Guid().ToString());

            var actionResult = await _controller.GetDtoListByUserIdAsync(testUserId);
            var objectResult = actionResult.Result as ObjectResult;
            var result = (ApiResponse<List<PrivateChatGroupsDto>>)objectResult!.Value!;

            Assert.True(objectResult!.StatusCode == 403);
            Assert.Equal(ErrorMessages.InvalidUserId, result.Message);
        }

        [Fact]
        public async Task GetDtoByGroupIdAsync_IsSuccess()
        {
            ApiResponse<PrivateChatGroupsDto> apiResponse = new();
            PrivateChatGroupsDto dto = GetDto();

            var _controller = GetNewController();

            _mockUserProvider.Setup(jwt => jwt.GetUserIdClaim(_controller.HttpContext))
                .Returns(dto.GroupOwnerUserId);
            _mockUserProvider.Setup(ip => ip.GetUserIP(_controller.HttpContext))
                .Returns(LocalHost);
            _mockService.Setup(g => g.GetDtoByGroupIdAsync(dto.ChatGroupId, dto.GroupOwnerUserId))
                .ReturnsAsync(ReturnApiResponse.Success(apiResponse, dto));

            var actionResult = await _controller.GetDtoByGroupIdAsync(dto.ChatGroupId, dto.GroupOwnerUserId);
            var objectResult = actionResult.Result as OkObjectResult;
            var result = (ApiResponse<PrivateChatGroupsDto>)objectResult!.Value!;

            Assert.Multiple(() =>
            {
                Assert.True(result.Success);
                Assert.Equal(dto.ChatGroupName, result.Data?.ChatGroupName);
            });
        }

        [Fact]
        public async Task AddAsync_IsSuccess()
        {
            ApiResponse<PrivateChatGroupsDto> apiResponse = new();
            CreatePrivateChatGroupDto createDto = new()
            {
                ChatGroupName    = LocalHost,
                GroupOwnerUserId = Guid.NewGuid().ToString()
            };

            var _controller = GetNewController();

            _mockUserProvider.Setup(id => id.GetUserIdClaim(_controller.HttpContext))
                .Returns(createDto.GroupOwnerUserId);
            _mockUserProvider.Setup(ip => ip.GetUserIP(_controller.HttpContext))
                .Returns(LocalHost);
            _mockService.Setup(a => a.AddAsync(createDto))
                .ReturnsAsync(ReturnApiResponse.Success(apiResponse, new()));

            var actionResult = await _controller.AddAsync(createDto);
            var objectResult = actionResult.Result as OkObjectResult;
            var result = (ApiResponse<PrivateChatGroupsDto>)objectResult!.Value!;

            Assert.True(result.Success);
        }

        [Fact]
        public async Task AddMemberAsync_IsSuccess()
        {
            ApiResponse<PrivateGroupMembers> apiResponse = new();
            int groupId = 1;
            string userId = Guid.NewGuid().ToString();
            PrivateGroupMembers newMember = new()
            {
                PrivateGroupMemberId = 99,
                PrivateChatGroupId = groupId,
                UserId = userId
            };

            var _controller = GetNewController();

            _mockUserProvider.Setup(ip => ip.GetUserIP(_controller.HttpContext))
               .Returns(LocalHost);
            _mockService.Setup(a => a.AddPrivateGroupMember(groupId, userId))
                .ReturnsAsync(ReturnApiResponse.Success(apiResponse, newMember));

            var actionResult = await _controller.AddMemberAsync(groupId, userId);
            var objectResult = actionResult.Result as OkObjectResult;
            var result = (ApiResponse<PrivateGroupMembers>)objectResult!.Value!;

            Assert.Multiple(() =>
            {
                Assert.True(result.Success);
                Assert.Equal(userId, result.Data?.UserId);
            });
        }

        [Fact]
        public async Task ModifyAsync_IsSuccess()
        {
            ApiResponse<PrivateChatGroupsDto> apiResponse = new();
            PrivateChatGroupsDto dto = GetDto();
            string modifiedName = "Modified";
            ModifyPrivateChatGroupDto modifyDto = new()
            {
                ChatGroupId   = dto.ChatGroupId,
                ChatGroupName = modifiedName
            };

            var _controller = GetNewController();

            _mockUserProvider.Setup(jwt => jwt.GetUserIdClaim(_controller.HttpContext))
                .Returns(dto.GroupOwnerUserId);
            _mockUserProvider.Setup(ip => ip.GetUserIP(_controller.HttpContext))
                .Returns(LocalHost);
            dto.ChatGroupName = modifiedName;
            _mockService.Setup(m => m.ModifyAsync(modifyDto, dto.GroupOwnerUserId))
                .ReturnsAsync(ReturnApiResponse.Success(apiResponse, dto));

            var actionResult = await _controller.ModifyAsync(modifyDto);
            var objectResult = actionResult.Result as OkObjectResult;
            var result = (ApiResponse<PrivateChatGroupsDto>)objectResult!.Value!;

            Assert.Multiple(() =>
            {
                Assert.True(result.Success);
                Assert.Equal(modifiedName, result.Data?.ChatGroupName);
            });
        }

        [Fact]
        public async Task DeleteAsync_IsSuccess()
        {
            ApiResponse<PrivateChatGroupsDto> apiResponse = new();
            PrivateChatGroupsDto dtoToDelete = GetDto();

            var _controller = GetNewController();

            _mockUserProvider.Setup(jwt => jwt.GetUserIdClaim(_controller.HttpContext))
                .Returns(dtoToDelete.GroupOwnerUserId);
            _mockUserProvider.Setup(ip => ip.GetUserIP(_controller.HttpContext))
                .Returns(LocalHost);
            _mockService.Setup(d => d.DeleteAsync(dtoToDelete.ChatGroupId, dtoToDelete.GroupOwnerUserId))
                .ReturnsAsync(ReturnApiResponse.Success(apiResponse, new()));

            var actionResult = await _controller.DeleteAsync(dtoToDelete.ChatGroupId);
            var objectResult = actionResult.Result as NoContentResult;

            Assert.True(objectResult!.StatusCode == 204);
        }

        [Fact]
        public async Task DeleteMemberAsync_IsSuccess()
        {
            ApiResponse<PrivateGroupMembers> apiResponse = new();
            int groupId = 1;
            string userId = Guid.NewGuid().ToString();

            var _controller = GetNewController();

            _mockUserProvider.Setup(ip => ip.GetUserIP(_controller.HttpContext))
               .Returns(LocalHost);
            _mockService.Setup(d => d.RemoveUserFromGroupAsync(groupId, userId))
                .ReturnsAsync(ReturnApiResponse.Success(apiResponse, new()));

            var actionResult = await _controller.DeleteMemberAsync(groupId, userId);
            var objectResult = actionResult.Result as NoContentResult;

            Assert.True(objectResult!.StatusCode == 204);
        }

        #region PRIVATE METHODS

        private PrivateChatGroupsController GetNewController()
        {
            return new(_mockService.Object, _mockSerilloger.Object, _mockUserProvider.Object);
        }

        private PrivateChatGroupsDto GetDto()
        {
            return new()
            {
                ChatGroupId      = 1,
                ChatGroupName    = "TestGroup",
                GroupCreated     = DateTime.Now,
                GroupOwnerUserId = Guid.NewGuid().ToString(),
                UserName         = "TestUser"
            };
        }

        #endregion
    }
}
