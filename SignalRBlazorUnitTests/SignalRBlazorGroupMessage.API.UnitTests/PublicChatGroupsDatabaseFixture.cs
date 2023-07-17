using ChatApplicationModels;
using Microsoft.EntityFrameworkCore;
using SignalRBlazorGroupsMessages.API.Data;
using SignalRBlazorGroupsMessages.API.Models.Views;

namespace SignalRBlazorUnitTests.SignalRBlazorGroupMessage.API.UnitTests
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
                        context.AddRange(GetListPublicChatGroupsViews());

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

            public virtual DbSet<PublicChatGroupsView> ChatGroupsViews { get; set; }
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

        private List<PublicChatGroups> GetListPublicChatGroups()
        {
            List<PublicChatGroups> chatGroupList = new()
            {
                new()
                {
                    //ChatGroupId      = 1,
                    ChatGroupName    = "TestPublicGroup1",
                    GroupCreated     = DateTime.Now,
                    GroupOwnerUserId = Guid.Parse("93eeda54-e362-49b7-8fd0-ab516b7f8071")
                },
                new()
                {
                    //ChatGroupId      = 2,
                    ChatGroupName    = "TestPublicGroup2",
                    GroupCreated     = DateTime.Now,
                    GroupOwnerUserId = Guid.Parse("93eeda54-e362-49b7-8fd0-ab516b7f8071")
                },
                new()
                {
                    //ChatGroupId      = 3,
                    ChatGroupName    = "TestPrivateGroup1",
                    GroupCreated     = DateTime.Now,
                    GroupOwnerUserId = Guid.Parse("93eeda54-e362-49b7-8fd0-ab516b7f8071")
                },
                new()
                {
                    //ChatGroupId      = 4,
                    ChatGroupName    = "TestPrivateGroup2",
                    GroupCreated     = DateTime.Now,
                    GroupOwnerUserId = Guid.Parse("93eeda54-e362-49b7-8fd0-ab516b7f8071")
                }
            };
            return chatGroupList;
        }

        private List<PublicChatGroupsView> GetListPublicChatGroupsViews()
        {
            List<PublicChatGroupsView> listView = new()
            {
                 new()
                {
                    //ChatGroupId      = 1,
                    ChatGroupName    = "TestPublicGroup1",
                    GroupCreated     = DateTime.Now,
                    GroupOwnerUserId = Guid.Parse("93eeda54-e362-49b7-8fd0-ab516b7f8071"),
                    UserName         = "Test Admin"
                },
                new()
                {
                    //ChatGroupId      = 2,
                    ChatGroupName    = "TestPublicGroup2",
                    GroupCreated     = DateTime.Now,
                    GroupOwnerUserId = Guid.Parse("93eeda54-e362-49b7-8fd0-ab516b7f8071"),
                    UserName         = "Test Admin"
                },
                new()
                {
                    //ChatGroupId      = 3,
                    ChatGroupName    = "TestPrivateGroup1",
                    GroupCreated     = DateTime.Now,
                    GroupOwnerUserId = Guid.Parse("93eeda54-e362-49b7-8fd0-ab516b7f8071"),
                    UserName         = "Test Admin"
                },
                new()
                {
                    //ChatGroupId      = 4,
                    ChatGroupName    = "TestPrivateGroup2",
                    GroupCreated     = DateTime.Now,
                    GroupOwnerUserId = Guid.Parse("93eeda54-e362-49b7-8fd0-ab516b7f8071"),
                    UserName         = "Test Admin"
                }
            };
            return listView;
        }
    }
}