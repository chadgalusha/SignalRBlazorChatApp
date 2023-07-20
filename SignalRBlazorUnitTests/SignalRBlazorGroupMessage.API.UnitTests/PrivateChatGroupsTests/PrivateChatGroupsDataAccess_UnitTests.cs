namespace SignalRBlazorUnitTests.SignalRBlazorGroupMessage.API.UnitTests.PrivateChatGroups
{
    public class PrivateChatGroupsDataAccess_UnitTests
    {
        //[Fact]
        //public async Task AddUserToPrivateChatGroup_AddsUser()
        //{
        //    int groupToJoinId = 4;
        //    string userIdToJoin = "e08b0077-3c15-477e-84bb-bf9d41196455";
        //    PrivateGroupMembers newPrivateGroupMember = GetNewPrivateGroupMember(groupToJoinId, userIdToJoin);

        //    _context.Database.BeginTransaction();
        //    bool resultOfAdd = await _dataAccess.AddUserToPrivateChatGroupAsync(newPrivateGroupMember);
        //    _context.ChangeTracker.Clear();

        //    List<PrivateGroupMembers> listPrivateGroupMembers = _context.PrivateGroupsMembers
        //        .Where(c => c.UserId == userIdToJoin)
        //        .ToList();
        //    bool isUserInGroup = _context.PrivateGroupsMembers
        //        .Where(p => p.PrivateChatGroupId == groupToJoinId
        //            && p.UserId == userIdToJoin)
        //        .Any();

        //    Assert.Multiple(() =>
        //    {
        //        Assert.True(resultOfAdd);
        //        Assert.Equal(2, listPrivateGroupMembers.Count);
        //        Assert.True(isUserInGroup);
        //    });
        //}

        //[Fact]
        //public async void GetPrivateChatGroupsByUserId_ReturnsCorrectList()
        //{
        //    Guid userId = Guid.Parse("e08b0077-3c15-477e-84bb-bf9d41196455");
        //    List<PublicChatGroupsView> listPrivateChatGroups = GetListPrivateChatGroups(userId);

        //    Mock<IPublicChatGroupsDataAccess> _mockDataAccess = new();
        //    _mockDataAccess.Setup(x => x.GetViewListPrivateByUserIdAsync(userId))
        //        .ReturnsAsync(listPrivateChatGroups);

        //    var mockedDataAccess = _mockDataAccess.Object;
        //    var result = await mockedDataAccess
        //        .GetViewListPrivateByUserIdAsync(userId);

        //    Assert.Equal(listPrivateChatGroups, result);
        //}

        //#region PRIVATE METHODS

        //private PrivateGroupMembers GetNewPrivateGroupMember(int groupToJoinId, string userIdToJoin)
        //{
        //    return new()
        //    {
        //        PrivateChatGroupId = groupToJoinId,
        //        UserId = userIdToJoin
        //    };
        //}

        //// Mock of stored procedure sp_getPrivateChatGroupsForUser @UserId
        //private List<PublicChatGroupsView> GetListPrivateChatGroups(Guid userId)
        //{
        //    List<PrivateGroupMembers> listPrivateGroupMembers = _context.PrivateGroupsMembers
        //        .Where(p => p.UserId == userId.ToString())
        //        .ToList();

        //    List<PublicChatGroupsView> listPrivateChatGroups = new();
        //    foreach (var listItem in listPrivateGroupMembers)
        //    {
        //        PublicChatGroups chatGroup = _context.PublicChatGroups
        //            .Single(c => c.ChatGroupId == listItem.PrivateChatGroupId);

        //        if (chatGroup != null)
        //        {
        //            listPrivateChatGroups.Add(ChatGroupToView(chatGroup));
        //        }
        //    }

        //    return listPrivateChatGroups;
        //}

        //#endregion
    }
}
