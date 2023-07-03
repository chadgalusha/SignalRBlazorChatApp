using ChatApplicationModels;

namespace SignalRBlazorGroupsMessages.API.Models
{
    public class ListPublicMessagesDto
    {
        public ICollection<PublicMessages> PublicMessages { get; set; } = new List<PublicMessages>();
        public int CurrentItemsCount { get; set; }
    }
}
