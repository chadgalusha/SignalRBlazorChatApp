using ChatApplicationModels;
using Microsoft.EntityFrameworkCore;
using SignalRBlazorGroupsMessages.API.Data;

namespace SignalRBlazorUnitTests.SignalRBlazorGroupMessage.API.UnitTests.PrivateMessages
{
    public class PrivateMessagesDatabaseFixture
    {
        private const string ConnectionString = @"Server=(localdb)\mssqllocaldb;Database=PrivateMessagesTestSample;Trusted_Connection=True";
        private static readonly object _lock = new();
        private static bool _databaseInitialized;

        public PrivateMessagesDatabaseFixture()
        {
            lock (_lock)
            {
                if (!_databaseInitialized)
                {
                    using (var context = CreateContext())
                    {
                        context.Database.EnsureDeleted();
                        context.Database.EnsureCreated();

                        context.AddRange(GetListPrivateMessages());

                        context.SaveChanges();
                    }

                    _databaseInitialized = true;
                }
            }
        }

        public ApplicationDbContext CreateContext()
        => new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(ConnectionString)
                .Options);

        private List<PrivateUserMessages> GetListPrivateMessages()
        {
            List<PrivateUserMessages> listPrivateMessages = new()
            {
                new()
                {
                    FromUserId             = "93eeda54-e362-49b7-8fd0-ab516b7f8071",
                    ToUserId               = "e08b0077-3c15-477e-84bb-bf9d41196455",
                    MessageText            = "Test Message 1",
                    MessageSeen            = false,
                    PrivateMessageDateTime = DateTime.Now
                },
                new()
                {
                    FromUserId             = "93eeda54-e362-49b7-8fd0-ab516b7f8071",
                    ToUserId               = "e08b0077-3c15-477e-84bb-bf9d41196455",
                    MessageText            = "Test Message 2",
                    MessageSeen            = false,
                    PrivateMessageDateTime = DateTime.Now
                },
                new()
                {
                    FromUserId             = "0a04b388-6e88-4d03-bcf9-4ac0f1893e55",
                    ToUserId               = "e08b0077-3c15-477e-84bb-bf9d41196455",
                    MessageText            = "Test Message 3",
                    MessageSeen            = false,
                    PrivateMessageDateTime = DateTime.Now
                },
                new()
                {
                    FromUserId             = "2973b407-752e-4c6a-917c-f5e43ef98597",
                    ToUserId               = "93eeda54-e362-49b7-8fd0-ab516b7f8071",
                    MessageText            = "Test Message 4",
                    MessageSeen            = false,
                    PrivateMessageDateTime = DateTime.Now
                }
            };
            return listPrivateMessages;
        }
    }
}
