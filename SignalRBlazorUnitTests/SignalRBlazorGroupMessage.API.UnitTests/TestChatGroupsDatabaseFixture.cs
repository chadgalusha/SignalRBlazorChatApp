using ChatApplicationModels;
using Microsoft.EntityFrameworkCore;
using SignalRBlazorGroupsMessages.API.Data;
using SignalRBlazorGroupsMessages.API.Models;

namespace SignalRBlazorUnitTests.SignalRBlazorGroupMessage.API.UnitTests
{
    public class TestChatGroupsDatabaseFixture
    {
        private const string ConnectionString = @"Server=(localdb)\mssqllocaldb;Database=ChatGroupsTestSample;Trusted_Connection=True";

        private static readonly object _lock = new();
        private static bool _databaseInitialized;

        public TestChatGroupsDatabaseFixture()
        {
            lock (_lock)
            {
                if (!_databaseInitialized)
                {
                    using (var context = CreateContext())
                    {
                        context.Database.EnsureDeleted();
                        context.Database.EnsureCreated();

                        context.AddRange(GetListChatGroups());
                        context.AddRange(GetListPrivateGroupMembers());
                        context.AddRange(GetListChatGroupsViews());

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

            public virtual DbSet<ChatGroupsView> ChatGroupsViews { get; set; }
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

        private List<ChatGroups> GetListChatGroups()
        {
            List<ChatGroups> chatGroupList = new()
            {
                new()
                {
                    //ChatGroupId      = 1,
                    ChatGroupName    = "TestPublicGroup1",
                    GroupCreated     = DateTime.Now,
                    GroupOwnerUserId = "93eeda54-e362-49b7-8fd0-ab516b7f8071",
                    PrivateGroup     = false
                },
                new()
                {
                    //ChatGroupId      = 2,
                    ChatGroupName    = "TestPublicGroup2",
                    GroupCreated     = DateTime.Now,
                    GroupOwnerUserId = "93eeda54-e362-49b7-8fd0-ab516b7f8071",
                    PrivateGroup     = false
                },
                new()
                {
                    //ChatGroupId      = 3,
                    ChatGroupName    = "TestPrivateGroup1",
                    GroupCreated     = DateTime.Now,
                    GroupOwnerUserId = "93eeda54-e362-49b7-8fd0-ab516b7f8071",
                    PrivateGroup     = true
                },
                new()
                {
                    //ChatGroupId      = 4,
                    ChatGroupName    = "TestPrivateGroup2",
                    GroupCreated     = DateTime.Now,
                    GroupOwnerUserId = "93eeda54-e362-49b7-8fd0-ab516b7f8071",
                    PrivateGroup     = true
                }
            };
            return chatGroupList;
        }

        private List<PrivateGroupMembers> GetListPrivateGroupMembers()
        {
            List<PrivateGroupMembers> privateGroupMembersList = new()
            {
                new()
                {
                    //PrivateGroupMemberId = 1,
                    PrivateChatGroupId   = 3,
                    UserId               = "93eeda54-e362-49b7-8fd0-ab516b7f8071"
                },
                new()
                {
                    //PrivateGroupMemberId = 2,
                    PrivateChatGroupId   = 4,
                    UserId               = "93eeda54-e362-49b7-8fd0-ab516b7f8071"
                },
                new()
                {
                    //PrivateGroupMemberId = 3,
                    PrivateChatGroupId   = 3,
                    UserId               = "e08b0077-3c15-477e-84bb-bf9d41196455"
                }
            };
            return privateGroupMembersList;
        }

        private List<ChatGroupsView> GetListChatGroupsViews()
        {
            List<ChatGroupsView> listView = new()
            {
                 new()
                {
                    //ChatGroupId      = 1,
                    ChatGroupName    = "TestPublicGroup1",
                    GroupCreated     = DateTime.Now,
                    GroupOwnerUserId = Guid.Parse("93eeda54-e362-49b7-8fd0-ab516b7f8071"),
                    UserName         = "Test Admin",
                    PrivateGroup     = false
                },
                new()
                {
                    //ChatGroupId      = 2,
                    ChatGroupName    = "TestPublicGroup2",
                    GroupCreated     = DateTime.Now,
                    GroupOwnerUserId = Guid.Parse("93eeda54-e362-49b7-8fd0-ab516b7f8071"),
                    UserName         = "Test Admin",
                    PrivateGroup     = false
                },
                new()
                {
                    //ChatGroupId      = 3,
                    ChatGroupName    = "TestPrivateGroup1",
                    GroupCreated     = DateTime.Now,
                    GroupOwnerUserId = Guid.Parse("93eeda54-e362-49b7-8fd0-ab516b7f8071"),
                    UserName         = "Test Admin",
                    PrivateGroup     = true
                },
                new()
                {
                    //ChatGroupId      = 4,
                    ChatGroupName    = "TestPrivateGroup2",
                    GroupCreated     = DateTime.Now,
                    GroupOwnerUserId = Guid.Parse("93eeda54-e362-49b7-8fd0-ab516b7f8071"),
                    UserName         = "Test Admin",
                    PrivateGroup     = true
                }
            };
            return listView;
        }
    }
}