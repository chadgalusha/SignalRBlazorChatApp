using ChatApplicationModels;

namespace SignalRBlazorGroupsMessages.API.Models
{
    public class ListPublicMessagesDto
    {
        public ICollection<PublicMessageDto> PublicMessages { get; set; } = new List<PublicMessageDto>();
        public int CurrentItemsCount { get; set; }
    }
}
