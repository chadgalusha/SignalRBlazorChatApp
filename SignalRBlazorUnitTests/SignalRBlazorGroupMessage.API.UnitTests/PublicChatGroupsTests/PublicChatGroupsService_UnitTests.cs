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
        public async Task GetListAsync_ReturnsCorrectResult()
        {
            int excpectedCount = GetDtoList().Count;

            _mockDataAccess.Setup(p => p.GetDtoListAsync())
                .ReturnsAsync(GetDtoList());

            PublicChatGroupsService _service = GetNewService();

            var result = await _service.GetListAsync();

            Assert.Multiple(() =>
            {
                Assert.Equal(excpectedCount, result.Data?.Count);
                Assert.True(result.Success);
                Assert.True(result.Message == "ok");
            });
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsCorrectResult()
        {
            PublicChatGroupsDto dto = GetDtoList().First();
            int groupId = dto.ChatGroupId;

            _mockDataAccess.Setup(p => p.GetDtoByIdAsync(1))
                .ReturnsAsync(GetDtoList()
                    .Single(p => p.ChatGroupId == groupId));
            _mockDataAccess.Setup(p => p.GroupExists(groupId))
                .Returns(true);

            PublicChatGroupsService _service = GetNewService();

            var result = await _service.GetByIdAsync(groupId);

            Assert.Multiple(() =>
            {
                Assert.True(result.Success);
                Assert.NotNull(result.Data);
            });
        }

        [Fact]
        public async Task AddAsync_IsSuccess()
        {
            CreatePublicChatGroupDto createDto = GetNewGroup();
            string expectedGroupName = createDto.ChatGroupName;
            var createdGroup = CreatedGroup();

            _mockDataAccess.Setup(p => p.GroupNameTaken(createDto.ChatGroupName))
                .Returns(false);
            _mockDataAccess.Setup(p => p.AddAsync(It.IsAny<ChatApplicationModels.PublicChatGroups>()))
                .ReturnsAsync(true);
            _mockDataAccess.Setup(g => g.GetDtoByIdAsync(createdGroup.ChatGroupId))
                .ReturnsAsync(createdGroup);

            PublicChatGroupsService _service = GetNewService();

            var result = await _service.AddAsync(createDto);

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
            var groupToModify = GetDtoList().First();
            string jwtUserId = groupToModify.GroupOwnerUserId;
            ModifyPublicChatGroupDto modifiedDto = DtoToModifyDto(groupToModify);
            modifiedDto.ChatGroupName = modifiedGroupName;

            _mockDataAccess.Setup(g => g.GetByGroupId(groupToModify.ChatGroupId))
                .Returns(DtoToModel(groupToModify));
            _mockDataAccess.Setup(p => p.GroupNameTaken(modifiedGroupName))
                .Returns(false);
            _mockDataAccess.Setup(p => p.ModifyAsync(It.IsAny<ChatApplicationModels.PublicChatGroups>()))
                .ReturnsAsync(true);
            // Modify chat group name here to mimic changed property.
            groupToModify.ChatGroupName = modifiedGroupName;
            _mockDataAccess.Setup(g => g.GetDtoByIdAsync(groupToModify.ChatGroupId))
                .ReturnsAsync(groupToModify);

            PublicChatGroupsService _service = GetNewService();

            var result = await _service.ModifyAsync(modifiedDto, jwtUserId);

            Assert.Multiple(() =>
            {
                Assert.True(result.Success);
                Assert.Equal(modifiedGroupName, result.Data?.ChatGroupName);
            });
        }

        [Fact]
        public async Task DeleteAsync_IsSuccess()
        {
            PublicChatGroupsDto groupToDelete = GetDtoList().First();
            int idToDelete = groupToDelete.ChatGroupId;
            string jwtUserId = groupToDelete.GroupOwnerUserId;

            _mockDataAccess.Setup(p => p.GetByGroupId(idToDelete))
                .Returns(DtoToModel(groupToDelete));
            _mockDataAccess.Setup(p => p.DeleteAsync(It.IsAny<ChatApplicationModels.PublicChatGroups>()))
                .ReturnsAsync(true);
            _mockMessagesService.Setup(p => p.DeleteAllMessagesInGroupAsync(idToDelete))
                .ReturnsAsync(true);

            PublicChatGroupsService _service = GetNewService();

            var result = await _service.DeleteAsync(idToDelete, jwtUserId);

            Assert.Multiple(() =>
            {
                Assert.True(result.Success);
                Assert.True(result.Message == "ok");
            });
        }

        #region PRIVATE METHODS

        private PublicChatGroupsService GetNewService()
        {
            return new(_mockDataAccess.Object, _mockMessagesService.Object, _mockSerilogger.Object);
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

        private CreatePublicChatGroupDto GetNewGroup()
        {
            return new()
            {
                ChatGroupName    = "Test Group 5",
                GroupOwnerUserId = "e1b9cf9a-ff86-4607-8765-9e47a305062a"
            };
        }

        private PublicChatGroupsDto CreatedGroup()
        {
            return new()
            {
                ChatGroupId      = 0,
                ChatGroupName    = "Test Group 5",
                GroupCreated     = DateTime.Now,
                GroupOwnerUserId = "e1b9cf9a-ff86-4607-8765-9e47a305062a",
                UserName         = "Test Owner"
            };
        }

        private ChatApplicationModels.PublicChatGroups DtoToModel(PublicChatGroupsDto dto)
        {
            return new()
            {
                ChatGroupId      = dto.ChatGroupId,
                ChatGroupName    = dto.ChatGroupName,
                GroupCreated     = dto.GroupCreated,
                GroupOwnerUserId = dto.GroupOwnerUserId
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
