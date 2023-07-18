using ChatApplicationModels;
using Microsoft.EntityFrameworkCore;
using SignalRBlazorGroupsMessages.API.Data;
using SignalRBlazorGroupsMessages.API.Models.Views;

namespace SignalRBlazorUnitTests.SignalRBlazorGroupMessage.API.UnitTests
{
    public class PublicMessagesDatabaseFixture
    {
        private const string ConnectionString = @"Server=(localdb)\mssqllocaldb;Database=PublicMessagesTestSample;Trusted_Connection=True";

        private static readonly object _lock = new();
        private static bool _databaseInitialized;

        public PublicMessagesDatabaseFixture()
        {
            lock (_lock)
            {
                if (!_databaseInitialized)
                {
                    using (var context = CreateContext())
                    {
                        context.Database.EnsureDeleted();
                        context.Database.EnsureCreated();

                        context.AddRange(GetListPublicMessages());
                        context.AddRange(GetListPublicMessagesViews());
                        context.SaveChanges();
                    }

                    _databaseInitialized = true;
                }
            }
        }

        // This class mocks results sent from stored procedures from PublicMessages in the production database
        public class TestPublicMessagesDbContext : ApplicationDbContext
        {
            public TestPublicMessagesDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
            public virtual DbSet<PublicGroupMessagesView> PublicMessagesView { get; set; }
        }

        //public ApplicationDbContext CreateContext()
        //    => new ApplicationDbContext(
        //        new DbContextOptionsBuilder<ApplicationDbContext>()
        //            .UseSqlServer(ConnectionString)
        //            .Options);

        public TestPublicMessagesDbContext CreateContext()
            => new TestPublicMessagesDbContext(
                new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseSqlServer(ConnectionString)
                    .Options);

        private List<PublicGroupMessages> GetListPublicMessages()
        {
            List<PublicGroupMessages> messageList = new()
            {
                new()
                {
                    PublicMessageId = Guid.Parse("e8ee70b6-678a-4b86-934e-da7f404a33a3"),
                    UserId          = "e1b9cf9a-ff86-4607-8765-9e47a305062a",
                    ChatGroupId     = 1,
                    Text            = "Sample message",
                    MessageDateTime = new DateTime(2023, 6, 15)
                },
                new()
                {
                    PublicMessageId = Guid.Parse("c57b308b-ca1a-4b85-919a-b147db30fde0"),
                    UserId          = "4eb0c266-894a-4c09-a6e2-4a0fb72e9c1c",
                    ChatGroupId     = 1,
                    Text            = "Sample message",
                    MessageDateTime = new DateTime(2023, 6, 15),
                    ReplyMessageId  = Guid.Parse("e8ee70b6-678a-4b86-934e-da7f404a33a3")
                },
                new()
                {
                    PublicMessageId = Guid.Parse("512fce5e-865a-4e4d-b6fd-2a57fb86149e"),
                    UserId          = "feac8ce0-5a21-4b89-9e23-beee9df517bb",
                    ChatGroupId     = 2,
                    Text            = "Sample message",
                    MessageDateTime = new DateTime(2023, 6, 15)
                },
                new()
                {
                    PublicMessageId = Guid.Parse("3eea1c79-61fb-41e0-852b-ab790835c827"),
                    UserId          = "8bc5d23a-9c70-4ef2-b285-814e993ad471",
                    ChatGroupId     = 2,
                    Text            = "Sample message",
                    MessageDateTime = new DateTime(2023, 6, 15)
                }
            };
            return messageList;
        }

        private List<PublicGroupMessagesView> GetListPublicMessagesViews()
        {
            List<PublicGroupMessagesView> messageList = new()
        {
            new()
                {
                    PublicMessageId = Guid.Parse("e8ee70b6-678a-4b86-934e-da7f404a33a3"),
                    UserId          = Guid.Parse("e1b9cf9a-ff86-4607-8765-9e47a305062a"),
                    UserName        = "TestUser1",
                    ChatGroupId     = 1,
                    ChatGroupName   = "Test Chat Group 1",
                    Text            = "Sample message",
                    MessageDateTime = new DateTime(2023, 6, 15)
                },
                new()
                {
                    PublicMessageId = Guid.Parse("c57b308b-ca1a-4b85-919a-b147db30fde0"),
                    UserId          = Guid.Parse("4eb0c266-894a-4c09-a6e2-4a0fb72e9c1c"),
                    UserName        = "TestUser2",
                    ChatGroupId     = 1,
                    ChatGroupName   = "Test Chat Group 1",
                    Text            = "Sample message",
                    MessageDateTime = new DateTime(2023, 6, 15)
                },
                new()
                {
                    PublicMessageId = Guid.Parse("512fce5e-865a-4e4d-b6fd-2a57fb86149e"),
                    UserId          = Guid.Parse("feac8ce0-5a21-4b89-9e23-beee9df517bb"),
                    UserName        = "TestUser3",
                    ChatGroupId     = 2,
                    ChatGroupName   = "Test Chat Group 2",
                    Text            = "Sample message",
                    MessageDateTime = new DateTime(2023, 6, 15)
                },
                new()
                {
                    PublicMessageId = Guid.Parse("3eea1c79-61fb-41e0-852b-ab790835c827"),
                    UserId          = Guid.Parse("8bc5d23a-9c70-4ef2-b285-814e993ad471"),
                    UserName        = "TestUser4",
                    ChatGroupId     = 2,
                    ChatGroupName   = "Test Chat Group 2",
                    Text            = "Sample message",
                    MessageDateTime = new DateTime(2023, 6, 15)
                }
        };
            return messageList;
        }
    }
}