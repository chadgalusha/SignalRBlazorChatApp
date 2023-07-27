namespace SignalRBlazorGroupsMessages.API.Models
{
    public struct ErrorMessages
    {
        public ErrorMessages()
        {
        }

        public const string AddingItem         = "Error adding item.";
        public const string AddingUser         = "Error adding user.";
        public const string DeletingItem       = "Error deleting item.";
        public const string DeletingMessages   = "Error deleting messages from this group.";
        public const string Deletinguser       = "Error removing group members from this group.";
        public const string GroupNameTaken     = "Group name already taken.";
        public const string InvalidUserId      = "Requesting userId not valid for this request.";
        public const string ModifyingItem      = "Error modifying item.";
        public const string NoModification     = "No modification needed.";
        public const string RecordNotFound     = "Record not found.";
        public const string RemovingUser       = "Error removing user.";
        public const string RetrievingItems    = "Error retrieving item(s).";
        public const string UserAlreadyInGroup = "User already in the private group.";
    }
}
