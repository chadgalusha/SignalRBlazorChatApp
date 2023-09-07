using ChatApplicationModels;
using Moq;
using SignalRBlazorGroupsMessages.API.DataAccess;
using SignalRBlazorGroupsMessages.API.Helpers;
using SignalRBlazorGroupsMessages.API.Models.Dtos;
using SignalRBlazorGroupsMessages.API.Services;

namespace SignalRBlazorUnitTests.SignalRBlazorGroupMessage.API.UnitTests.PrivateChatGroups
{
    public class PrivateChatGroupsService_UnitTests
    {
        private readonly Mock<IPrivateChatGroupsDataAccess> _mockGroupsDataAccess;
        private readonly Mock<IPrivateGroupMessagesDataAccess> _mockMessagesDataAccess;
        private readonly Mock<ISerilogger> _mockSerilogger;

        public PrivateChatGroupsService_UnitTests()
        {
            _mockGroupsDataAccess = new Mock<IPrivateChatGroupsDataAccess>();
            _mockMessagesDataAccess = new Mock<IPrivateGroupMessagesDataAccess>();
            _mockSerilogger = new Mock<ISerilogger>();
        }

        [Fact]
        public async Task GetDtoListByUserIdAsync_ReturnsCorrectResult()
        {
            List<PrivateChatGroupsDto> dtoList = GetListDtos();
            string testUserId = Guid.NewGuid().ToString();

            _mockGroupsDataAccess.Setup(g => g.GetDtoListByUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync(dtoList);

            var _service = GetNewService();

            var result = await _service.GetDtoListByUserIdAsync(It.IsAny<string>());

            Assert.Multiple(() =>
            {
                Assert.True(result.Success);
                Assert.NotNull(result.Data);
            });
        }

        [Fact]
        public async Task GetDtoByGroupIdAsync_ReturnsCorrectResult()
        {
            PrivateChatGroupsDto dto = GetListDtos().First();
            int groupId = dto.ChatGroupId;
            string userId = dto.GroupOwnerUserId;

            _mockGroupsDataAccess.Setup(g => g.GroupExists(groupId))
                .ReturnsAsync(true);
            _mockGroupsDataAccess.Setup(i => i.IsUserInPrivateGroup(groupId, userId))
                .ReturnsAsync(true);
            _mockGroupsDataAccess.Setup(g => g.GetDtoByGroupIdAsync(groupId))
                .ReturnsAsync(dto);

            var _service = GetNewService();

            var result = await _service.GetDtoByGroupIdAsync(groupId, userId);

            Assert.Multiple(() =>
            {
                Assert.True(result.Success);
                Assert.Equal(dto, result.Data);
            });

        }

        [Fact]
        public async Task AddAsync_IsSuccess()
        {
            CreatePrivateChatGroupDto createDto = GetCreateDto();

            _mockGroupsDataAccess.Setup(g => g.GroupNameTaken(createDto.ChatGroupName))
                .Returns(false);
            _mockGroupsDataAccess.Setup(a => a.AddAsync(It.IsAny<ChatApplicationModels.PrivateChatGroups>()))
                .ReturnsAsync(true);
            _mockGroupsDataAccess.Setup(g => g.GetDtoByGroupIdAsync(It.IsAny<int>()))
                .ReturnsAsync(CreatedDto());

            var _service = GetNewService();

            var result = await _service.AddAsync(createDto);

            Assert.Multiple(() =>
            {
                Assert.True(result.Success);
                Assert.Equal(createDto.ChatGroupName, result.Data?.ChatGroupName);
            });
        }

        [Fact]
        public async Task ModifyAsync_IsSuccess()
        {
            PrivateChatGroupsDto dto = GetListDtos().First();
            string jwtUserId = dto.GroupOwnerUserId;
            ModifyPrivateChatGroupDto modifyDto = DtoToModify(dto);
            string modifiedName = "Modified";
            modifyDto.ChatGroupName = modifiedName;

            _mockGroupsDataAccess.Setup(g => g.GetByGroupId(modifyDto.ChatGroupId))
                .Returns(GetListPrivateChatGroups().First());
            _mockGroupsDataAccess.Setup(n => n.GroupNameTaken(modifyDto.ChatGroupName))
                .Returns(false);
            _mockGroupsDataAccess.Setup(m => m.ModifyAsync(It.IsAny<ChatApplicationModels.PrivateChatGroups>()))
                .ReturnsAsync(true);
            dto.ChatGroupName = modifiedName;
            _mockGroupsDataAccess.Setup(g => g.GetDtoByGroupIdAsync(dto.ChatGroupId))
                .ReturnsAsync(dto);

            var _service = GetNewService();

            var result = await _service.ModifyAsync(modifyDto, jwtUserId);

            Assert.Multiple(() =>
            {
                Assert.True(result.Success);
                Assert.Equal(modifiedName, result.Data?.ChatGroupName);
            });
        }

        [Fact]
        public async Task DeleteAsync_IsSuccess()
        {
            var groupToDelete = GetListPrivateChatGroups().First();
            int groupId = groupToDelete.ChatGroupId;
            string jwtUserId = groupToDelete.GroupOwnerUserId;

            _mockGroupsDataAccess.Setup(g => g.GetByGroupId(groupId))
                .Returns(groupToDelete);
            _mockMessagesDataAccess.Setup(d => d.DeleteAllMessagesInGroupAsync(groupId))
                .ReturnsAsync(true);
            _mockGroupsDataAccess.Setup(r => r.RemoveAllUsersFromGroupAsync(groupId))
                .ReturnsAsync(true);
            _mockGroupsDataAccess.Setup(d => d.DeleteAsync(groupToDelete))
                .ReturnsAsync(true);

            var _service = GetNewService();

            var result = await _service.DeleteAsync(groupId, jwtUserId);

            Assert.Multiple(() =>
            {
                Assert.True(result.Success);
                Assert.NotEqual(groupId, result.Data?.ChatGroupId);
            });
        }

        [Fact]
        public async Task AddPrivateGroupMember_IsSuccess()
        {
            int groupId = 4;
            string userId = GetListPrivateGroupMembers().First().UserId;

            _mockGroupsDataAccess.Setup(i => i.IsUserInPrivateGroup(groupId, userId))
                .ReturnsAsync(false);
            _mockGroupsDataAccess.Setup(a => a.AddUserToGroupAsync(It.IsAny<PrivateGroupMembers>()))
                .ReturnsAsync(true);

            var _service = GetNewService();

            var result = await _service.AddPrivateGroupMember(groupId, userId);

            Assert.True(result.Success);
        }

        [Fact]
        public async Task RemoveUserFromGroupAsync_IsSuccess()
        {
            PrivateGroupMembers member = GetListPrivateGroupMembers().First();
            int groupId = member.PrivateChatGroupId;
            string userId = member.UserId;

            _mockGroupsDataAccess.Setup(g => g.GetPrivateGroupMemberRecord(groupId, userId))
                .ReturnsAsync(member);
            _mockGroupsDataAccess.Setup(r => r.RemoveUserFromPrivateChatGroup(member))
                .ReturnsAsync(true);

            var _service = GetNewService();

            var result = await _service.RemoveUserFromGroupAsync(groupId, userId);

            Assert.True(result.Success);
        }

        #region PRIVATE METHODS

        private PrivateChatGroupsService GetNewService()
        {
            return new(_mockGroupsDataAccess.Object, _mockMessagesDataAccess.Object, _mockSerilogger.Object);
        }

        private List<ChatApplicationModels.PrivateChatGroups> GetListPrivateChatGroups()
        {
            List<ChatApplicationModels.PrivateChatGroups> chatGroupList = new()
            {
                new()
                {
                    ChatGroupId      = 1,
                    ChatGroupName    = "TestPrivateGroup1",
                    GroupCreated     = new DateTime(2023, 7, 23),
                    GroupOwnerUserId = "93eeda54-e362-49b7-8fd0-ab516b7f8071"
                },
                new()
                {
                    ChatGroupId      = 2,
                    ChatGroupName    = "TestPrivateGroup2",
                    GroupCreated     = new DateTime(2023, 7, 23),
                    GroupOwnerUserId = "93eeda54-e362-49b7-8fd0-ab516b7f8071"
                },
                new()
                {
                    ChatGroupId      = 3,
                    ChatGroupName    = "TestPrivateGroup3",
                    GroupCreated     = new DateTime(2023, 7, 23),
                    GroupOwnerUserId = "93eeda54-e362-49b7-8fd0-ab516b7f8071"
                },
                new()
                {
                    ChatGroupId      = 4,
                    ChatGroupName    = "TestPrivateGroup4",
                    GroupCreated     = new DateTime(2023, 7, 23),
                    GroupOwnerUserId = "93eeda54-e362-49b7-8fd0-ab516b7f8071"
                }
            };
            return chatGroupList;
        }

        private List<PrivateChatGroupsDto> GetListDtos()
        {
            List<PrivateChatGroupsDto> listDto = new()
            {
                 new()
                {
                    ChatGroupId      = 1,
                    ChatGroupName    = "TestPrivateGroup1",
                    GroupCreated     = new DateTime(2023, 7, 23),
                    GroupOwnerUserId = "93eeda54-e362-49b7-8fd0-ab516b7f8071",
                    UserName         = "Test Admin"
                },
                new()
                {
                    ChatGroupId      = 2,
                    ChatGroupName    = "TestPrivateGroup2",
                    GroupCreated     = new DateTime(2023, 7, 23),
                    GroupOwnerUserId = "93eeda54-e362-49b7-8fd0-ab516b7f8071",
                    UserName         = "Test Admin"
                },
                new()
                {
                    ChatGroupId      = 3,
                    ChatGroupName    = "TestPrivateGroup3",
                    GroupCreated     = new DateTime(2023, 7, 23),
                    GroupOwnerUserId = "93eeda54-e362-49b7-8fd0-ab516b7f8071",
                    UserName         = "Test Admin"
                },
                new()
                {
                    ChatGroupId      = 4,
                    ChatGroupName    = "TestPrivateGroup4",
                    GroupCreated     = new DateTime(2023, 7, 23),
                    GroupOwnerUserId = "93eeda54-e362-49b7-8fd0-ab516b7f8071",
                    UserName         = "Test Admin"
                }
            };
            return listDto;
        }

        private List<PrivateGroupMembers> GetListPrivateGroupMembers()
        {
            return new()
            {
                new()
                {
                    PrivateGroupMemberId = 1,
                    PrivateChatGroupId   = 1,
                    UserId               = "93eeda54-e362-49b7-8fd0-ab516b7f8071"
                },
                new()
                {
                    PrivateGroupMemberId = 2,
                    PrivateChatGroupId   = 2,
                    UserId               = "93eeda54-e362-49b7-8fd0-ab516b7f8071"
                },
                new()
                {
                    PrivateGroupMemberId = 3,
                    PrivateChatGroupId   = 3,
                    UserId               = "93eeda54-e362-49b7-8fd0-ab516b7f8071"
                }
            };
        }

        private CreatePrivateChatGroupDto GetCreateDto()
        {
            return new()
            {
                ChatGroupName    = "Newgroup",
                GroupOwnerUserId = "93eeda54-e362-49b7-8fd0-ab516b7f8071"
            };
        }

        private PrivateChatGroupsDto CreatedDto()
        {
            return new()
            {
                ChatGroupId      = 5,
                ChatGroupName    = "Newgroup",
                GroupOwnerUserId = "93eeda54-e362-49b7-8fd0-ab516b7f8071",
                UserName         = "TestUser",
                GroupCreated     = new DateTime(2023, 7, 29)
            };
        }

        private ModifyPrivateChatGroupDto DtoToModify(PrivateChatGroupsDto dto)
        {
            return new()
            {
                ChatGroupId   = dto.ChatGroupId,
                ChatGroupName = dto.ChatGroupName
            };
        }

        #endregion
    }
}
