namespace SignalRBlazorGroupsMessages.API.Helpers
{
    public class GroupsMessagesExceptions : Exception
    {
        public GroupsMessagesExceptions() { }

        public GroupsMessagesExceptions(string message) : base(message) { }

        public GroupsMessagesExceptions(string message, Exception innerException) : base(message, innerException) { }
    }
}
