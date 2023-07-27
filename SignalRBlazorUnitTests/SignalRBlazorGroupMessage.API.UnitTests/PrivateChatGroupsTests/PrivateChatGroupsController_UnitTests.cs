using Microsoft.AspNetCore.Mvc;
using Moq;
using SignalRBlazorGroupsMessages.API.Controllers;
using SignalRBlazorGroupsMessages.API.Helpers;
using SignalRBlazorGroupsMessages.API.Models;
using SignalRBlazorGroupsMessages.API.Models.Dtos;
using SignalRBlazorGroupsMessages.API.Services;

namespace SignalRBlazorUnitTests.SignalRBlazorGroupMessage.API.UnitTests.PrivateChatGroupsTests
{
    public class PrivateChatGroupsController_UnitTests : ControllerBase
    {
        private readonly Mock<IPrivateChatGroupsService> _mockService;
        private readonly Mock<IUserProvider> _mockUserProvider;
        private readonly Mock<ISerilogger> _mockSerilloger;

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
            _mockUserProvider.Setup(ip => ip.GetUserIdClaim(_controller.HttpContext))
                .Returns(testUserId);
            _mockUserProvider.Setup(ip => ip.GetUserIP(_controller.HttpContext))
                .Returns("127.0.0.1");

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

        #region PRIVATE METHODS

        private PrivateChatGroupsController GetNewController()
        {
            return new(_mockService.Object, _mockSerilloger.Object, _mockUserProvider.Object);
        }

        #endregion
    }
}
