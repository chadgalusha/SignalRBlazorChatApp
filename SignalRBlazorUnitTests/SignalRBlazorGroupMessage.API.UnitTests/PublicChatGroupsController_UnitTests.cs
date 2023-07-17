using Moq;
using SignalRBlazorGroupsMessages.API.Controllers;
using SignalRBlazorGroupsMessages.API.Helpers;
using SignalRBlazorGroupsMessages.API.Models;
using SignalRBlazorGroupsMessages.API.Models.Dtos;
using SignalRBlazorGroupsMessages.API.Services;

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

            PublicChatGroupsController _controller = new(_mockService.Object, _mockSerilogger.Object);

            var result = await _controller.GetPublicChatGroupsAsync();

            Assert.Multiple(() =>
            {
                Assert.True(result.Value?.Success == true);
                Assert.NotNull(result.Value.Data);
            });
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsCorrectresult()
        {
            ApiResponse<PublicChatGroupsDto> response = new();
            PublicChatGroupsDto dto = GetDtoList().First();
            int id = dto.ChatGroupId;

            _mockService.Setup(p => p.GetViewByIdAsync(id))
                .ReturnsAsync(ReturnApiResponse.Success(response, dto));

            PublicChatGroupsController _controller = new(_mockService.Object, _mockSerilogger.Object);

            var result = await _controller.GetByIdAsync(id);

            Assert.Multiple(() =>
            {
                Assert.True(result.Value?.Success == true);
                Assert.NotNull(result.Value.Data);
            });
        }

        #region PRIVATE METHODS

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

        #endregion
    }
}
