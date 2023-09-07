using ChatApplicationModels;
using Microsoft.EntityFrameworkCore;
using SignalRBlazorGroupsMessages.API.Data;
using SignalRBlazorGroupsMessages.API.Models.Dtos;

namespace SignalRBlazorUnitTests.SignalRBlazorGroupMessage.API.UnitTests.PrivateChatGroups
{
    public class PrivateChatGroupsDatabaseFixture
    {
        private const string ConnectionString = @"Server=(localdb)\mssqllocaldb;Database=ChatGroupsTestSample;Trusted_Connection=True";

        private static readonly object _lock = new();
        private static bool _databaseInitialized;

        public PrivateChatGroupsDatabaseFixture()
        {
            lock (_lock)
            {
                if (!_databaseInitialized)
                {
                    using (var context = CreateContext())
                    {
                        context.Database.EnsureDeleted();
                        context.Database.EnsureCreated();

                        context.AddRange(GetListPrivateChatGroups());
                        context.AddRange(GetListDtos());
                        context.AddRange(GetListPrivateGroupMembers());

                        context.SaveChanges();
                    }
                    _databaseInitialized = true;
                }
            }
        }

        public class TestPrivateChatGroupDbContext : ApplicationDbContext
        {
            public TestPrivateChatGroupDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

            public virtual DbSet<PrivateChatGroupsDto> PrivateChatGroupsDto { get; set; }
        }

        public TestPrivateChatGroupDbContext CreateContext()
            => new TestPrivateChatGroupDbContext(
                new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(ConnectionString)
                .Options);

        private List<ChatApplicationModels.PrivateChatGroups> GetListPrivateChatGroups()
        {
            List<ChatApplicationModels.PrivateChatGroups> chatGroupList = new()
            {
                new()
                {
                    //ChatGroupId      = 1,
                    ChatGroupName    = "TestPrivateGroup1",
                    GroupCreated     = DateTime.Now,
                    GroupOwnerUserId = "93eeda54-e362-49b7-8fd0-ab516b7f8071"
                },
                new()
                {
                    //ChatGroupId      = 2,
                    ChatGroupName    = "TestPrivateGroup2",
                    GroupCreated     = DateTime.Now,
                    GroupOwnerUserId = "93eeda54-e362-49b7-8fd0-ab516b7f8071"
                },
                new()
                {
                    //ChatGroupId      = 3,
                    ChatGroupName    = "TestPrivateGroup3",
                    GroupCreated     = DateTime.Now,
                    GroupOwnerUserId = "93eeda54-e362-49b7-8fd0-ab516b7f8071"
                },
                new()
                {
                    //ChatGroupId      = 4,
                    ChatGroupName    = "TestPrivateGroup4",
                    GroupCreated     = DateTime.Now,
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
                    //ChatGroupId      = 1,
                    ChatGroupName    = "TestPublicGroup1",
                    GroupCreated     = DateTime.Now,
                    GroupOwnerUserId = "93eeda54-e362-49b7-8fd0-ab516b7f8071",
                    UserName         = "Test Admin"
                },
                new()
                {
                    //ChatGroupId      = 2,
                    ChatGroupName    = "TestPublicGroup2",
                    GroupCreated     = DateTime.Now,
                    GroupOwnerUserId = "93eeda54-e362-49b7-8fd0-ab516b7f8071",
                    UserName         = "Test Admin"
                },
                new()
                {
                    //ChatGroupId      = 3,
                    ChatGroupName    = "TestPrivateGroup3",
                    GroupCreated     = DateTime.Now,
                    GroupOwnerUserId = "93eeda54-e362-49b7-8fd0-ab516b7f8071",
                    UserName         = "Test Admin"
                },
                new()
                {
                    //ChatGroupId      = 4,
                    ChatGroupName    = "TestPrivateGroup4",
                    GroupCreated     = DateTime.Now,
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
                    PrivateChatGroupId = 1,
                    UserId = "93eeda54-e362-49b7-8fd0-ab516b7f8071"
                },
                new()
                {
                    PrivateChatGroupId = 2,
                    UserId = "93eeda54-e362-49b7-8fd0-ab516b7f8071"
                },
                new()
                {
                    PrivateChatGroupId = 3,
                    UserId = "93eeda54-e362-49b7-8fd0-ab516b7f8071"
                }
            };
        }
    }
}
