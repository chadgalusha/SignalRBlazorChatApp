using Microsoft.EntityFrameworkCore;
using SignalRBlazorGroupsMessages.API.Data;
using SignalRBlazorGroupsMessages.API.Models.Dtos;

namespace SignalRBlazorUnitTests.SignalRBlazorGroupMessage.API.UnitTests.PublicChatGroups
{
    public class PublicChatGroupsDatabaseFixture
    {
        private const string ConnectionString = @"Server=(localdb)\mssqllocaldb;Database=ChatGroupsTestSample;Trusted_Connection=True";

        private static readonly object _lock = new();
        private static bool _databaseInitialized;

        public PublicChatGroupsDatabaseFixture()
        {
            lock (_lock)
            {
                if (!_databaseInitialized)
                {
                    using (var context = CreateContext())
                    {
                        context.Database.EnsureDeleted();
                        context.Database.EnsureCreated();

                        context.AddRange(GetListPublicChatGroups());
                        context.AddRange(GetListPublicChatGroupsDtos());

                        context.SaveChanges();
                    }

                    _databaseInitialized = true;
                }
            }
        }

        // This class mocks results sent from stored procedures for ChatGroups in the production database
        public class TestChatGroupsDbContext : ApplicationDbContext
        {
            public TestChatGroupsDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

            public virtual DbSet<PublicChatGroupsDto> ChatGroupsDtos { get; set; }
        }

        public TestChatGroupsDbContext CreateContext()
            => new TestChatGroupsDbContext(
                new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseSqlServer(ConnectionString)
                    .Options);

        //public ApplicationDbContext CreateContext()
        //=> new ApplicationDbContext(
        //    new DbContextOptionsBuilder<ApplicationDbContext>()
        //        .UseSqlServer(ConnectionString)
        //        .Options);

        private List<ChatApplicationModels.PublicChatGroups> GetListPublicChatGroups()
        {
            List<ChatApplicationModels.PublicChatGroups> chatGroupList = new()
            {
                new()
                {
                    //ChatGroupId      = 1,
                    ChatGroupName    = "TestPublicGroup1",
                    GroupCreated     = DateTime.Now,
                    GroupOwnerUserId = "93eeda54-e362-49b7-8fd0-ab516b7f8071"
                },
                new()
                {
                    //ChatGroupId      = 2,
                    ChatGroupName    = "TestPublicGroup2",
                    GroupCreated     = DateTime.Now,
                    GroupOwnerUserId = "93eeda54-e362-49b7-8fd0-ab516b7f8071"
                },
                new()
                {
                    //ChatGroupId      = 3,
                    ChatGroupName    = "TestPublicGroup3",
                    GroupCreated     = DateTime.Now,
                    GroupOwnerUserId = "93eeda54-e362-49b7-8fd0-ab516b7f8071"
                },
                new()
                {
                    //ChatGroupId      = 4,
                    ChatGroupName    = "TestPublicGroup4",
                    GroupCreated     = DateTime.Now,
                    GroupOwnerUserId = "93eeda54-e362-49b7-8fd0-ab516b7f8071"
                }
            };
            return chatGroupList;
        }

        private List<PublicChatGroupsDto> GetListPublicChatGroupsDtos()
        {
            List<PublicChatGroupsDto> listDto = new()
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
                    ChatGroupName    = "TestPrivateGroup1",
                    GroupCreated     = DateTime.Now,
                    GroupOwnerUserId = "93eeda54-e362-49b7-8fd0-ab516b7f8071",
                    UserName         = "Test Admin"
                },
                new()
                {
                    //ChatGroupId      = 4,
                    ChatGroupName    = "TestPrivateGroup2",
                    GroupCreated     = DateTime.Now,
                    GroupOwnerUserId = "93eeda54-e362-49b7-8fd0-ab516b7f8071",
                    UserName         = "Test Admin"
                }
            };
            return listDto;
        }
    }
}