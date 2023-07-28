using Moq;
using SignalRBlazorGroupsMessages.API.DataAccess;
using SignalRBlazorGroupsMessages.API.Helpers;
using SignalRBlazorGroupsMessages.API.Models.Dtos;
using SignalRBlazorGroupsMessages.API.Services;

namespace SignalRBlazorUnitTests.SignalRBlazorGroupMessage.API.UnitTests.PublicChatGroups
{
    public class PublicChatGroupsService_UnitTests
    {
        private readonly Mock<IPublicChatGroupsDataAccess> _mockDataAccess;
        private readonly Mock<IPublicGroupMessagesService> _mockMessagesService;
        private readonly Mock<ISerilogger> _mockSerilogger;

        public PublicChatGroupsService_UnitTests()
        {
            _mockDataAccess = new Mock<IPublicChatGroupsDataAccess>();
            _mockMessagesService = new Mock<IPublicGroupMessagesService>();
            _mockSerilogger = new Mock<ISerilogger>();
        }

        [Fact]
        public async Task GetListPublicChatGroupsAsync_ReturnsCorrectResult()
        {
            int excpectedCount = GetDtoListPublicChatGroups().Count;

            _mockDataAccess.Setup(p => p.GetDtoListAsync())
                .ReturnsAsync(GetDtoListPublicChatGroups());

            PublicChatGroupsService _service = new(_mockDataAccess.Object, _mockMessagesService.Object, _mockSerilogger.Object);

            var result = await _service.GetListAsync();

            Assert.Multiple(() =>
            {
                Assert.Equal(excpectedCount, result.Data?.Count);
                Assert.True(result.Success);
                Assert.True(result.Message == "ok");
            });
        }

        [Fact]
        public async Task GetViewByIdAsync_ReturnsCorrectResult()
        {
            PublicChatGroupsDto dto = GetDtoListPublicChatGroups().First();

            _mockDataAccess.Setup(p => p.GetDtoByIdAsync(1))
                .ReturnsAsync(GetDtoListPublicChatGroups()
                    .Single(p => p.ChatGroupId == 1));
            _mockDataAccess.Setup(p => p.GroupExists(1))
                .Returns(true);

            PublicChatGroupsService _service = new(_mockDataAccess.Object, _mockMessagesService.Object, _mockSerilogger.Object);

            var goodResult = await _service.GetDtoByIdAsync(1);
            var badResult = await _service.GetDtoByIdAsync(999);

            Assert.Multiple(() =>
            {
                //Assert.Equal(1, goodResult.Data?.ChatGroupId);
                Assert.True(goodResult.Success);

                Assert.False(badResult.Success);
                Assert.Null(badResult.Data);
            });
        }

        [Fact]
        public async Task AddAsync_IsSuccess()
        {
            CreatePublicChatGroupDto newGroup = GetNewGroup();
            string expectedGroupName = newGroup.ChatGroupName;

            _mockDataAccess.Setup(p => p.GroupNameTaken(newGroup.ChatGroupName))
                .Returns(false);
            _mockDataAccess.Setup(p => p.AddAsync(It.IsAny<ChatApplicationModels.PublicChatGroups>()))
                .ReturnsAsync(true);

            PublicChatGroupsService _service = new(_mockDataAccess.Object, _mockMessagesService.Object, _mockSerilogger.Object);

            var result = await _service.AddAsync(newGroup);

            Assert.Multiple(() =>
            {
                Assert.True(result.Success);
                Assert.Equal(expectedGroupName, result.Data?.ChatGroupName);
            });
        }

        [Fact]
        public async Task ModifyAsync_IsSuccess()
        {
            string modifiedGroupName = "Modified Group Name";
            var groupToModify = GetDtoListPublicChatGroups().First();
            groupToModify.ChatGroupName = modifiedGroupName;
            ModifyPublicChatGroupDto modifiedDto = DtoToModifyDto(groupToModify);

            _mockDataAccess.Setup(p => p.GetDtoByIdAsync(groupToModify.ChatGroupId))
                .ReturnsAsync(
                    GetDtoListPublicChatGroups()
                        .First());
            _mockDataAccess.Setup(p => p.GroupNameTaken(modifiedGroupName))
                .Returns(false);
            _mockDataAccess.Setup(p => p.ModifyAsync(It.IsAny<ChatApplicationModels.PublicChatGroups>()))
                .ReturnsAsync(true);

            PublicChatGroupsService _service = new(_mockDataAccess.Object, _mockMessagesService.Object, _mockSerilogger.Object);

            var result = await _service.ModifyAsync(modifiedDto);

            Assert.Multiple(() =>
            {
                Assert.True(result.Success);
                Assert.Equal(modifiedGroupName, result.Data?.ChatGroupName);
            });
        }

        [Fact]
        public async Task DeleteAsync_IsSuccess()
        {
            PublicChatGroupsDto groupToDelete = GetDtoListPublicChatGroups().First();
            int idToDelete = groupToDelete.ChatGroupId;

            _mockDataAccess.Setup(p => p.GroupExists(idToDelete))
                .Returns(true);
            _mockDataAccess.Setup(p => p.GetByGroupId(idToDelete))
                .Returns(DtoToModel(groupToDelete));
            _mockDataAccess.Setup(p => p.DeleteAsync(It.IsAny<ChatApplicationModels.PublicChatGroups>()))
                .ReturnsAsync(true);
            _mockMessagesService.Setup(p => p.DeleteAllMessagesInGroupAsync(idToDelete))
                .ReturnsAsync(true);

            PublicChatGroupsService _service = new(_mockDataAccess.Object, _mockMessagesService.Object, _mockSerilogger.Object);

            var result = await _service.DeleteAsync(idToDelete);

            Assert.Multiple(() =>
            {
                Assert.True(result.Success);
                Assert.True(result.Message == "ok");
            });
        }

        #region PRIVATE METHODS

        private List<PublicChatGroupsDto> GetDtoListPublicChatGroups()
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

        private CreatePublicChatGroupDto GetNewGroup()
        {
            return new()
            {
                ChatGroupName = "Test Group 5",
                GroupOwnerUserId = Guid.Parse("e1b9cf9a-ff86-4607-8765-9e47a305062a")
            };
        }

        private PublicChatGroupsDto GetNewGroupDto(ChatApplicationModels.PublicChatGroups newGroup)
        {
            return new()
            {
                ChatGroupId = newGroup.ChatGroupId,
                ChatGroupName = newGroup.ChatGroupName,
                GroupCreated = newGroup.GroupCreated,
                GroupOwnerUserId = newGroup.GroupOwnerUserId,
                UserName = "Test Owner 2"
            };
        }

        private ChatApplicationModels.PublicChatGroups DtoToModel(PublicChatGroupsDto dto)
        {
            return new()
            {
                ChatGroupId = dto.ChatGroupId,
                ChatGroupName = dto.ChatGroupName,
                GroupCreated = dto.GroupCreated,
                GroupOwnerUserId = dto.GroupOwnerUserId.ToString()
            };
        }

        private ModifyPublicChatGroupDto DtoToModifyDto(PublicChatGroupsDto dto)
        {
            return new()
            {
                ChatGroupId = dto.ChatGroupId,
                ChatGroupName = dto.ChatGroupName
            };
        }

        #endregion
    }
}
