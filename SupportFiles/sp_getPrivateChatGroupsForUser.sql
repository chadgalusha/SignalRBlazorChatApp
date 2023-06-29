CREATE PROCEDURE sp_getPrivateChatGroupsForUser
	@UserId nvarchar(450)
AS
BEGIN
	SELECT *
	FROM ChatApplication.dbo.ChatGroups
	WHERE ChatGroupId IN (SELECT PrivateChatGroupId
						  FROM ChatApplication.dbo.PrivateGroupMembers
						  WHERE UserId = @UserId)
END