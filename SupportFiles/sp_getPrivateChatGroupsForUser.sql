-- ################################################################################# --
-- Stored Procedures for use in Data Access layer in SignalRBlazorGroupsMessages.API --
-- ################################################################################# --

-- 
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

-- PublicMessagesDataAccess.GetPublicMessageByIdAsync(string messageId)
CREATE OR ALTER PROCEDURE sp_getPublicMessage_byMessageId
	@messageId nvarchar(450)
AS
BEGIN
	SELECT 
		p.PublicMessageId, 
		p.UserId, 
		a.UserName, 
		p.ChatGroupId, 
		c.ChatGroupName, 
		p.Text, 
		p.MessageDateTime, 
		p.ReplyMessageId, 
		p.PictureLink
	FROM PublicMessages p 
	JOIN AspNetUsers a
		ON p.UserId = a.Id
	JOIN ChatGroups c
		ON p.ChatGroupId = c.ChatGroupId
	WHERE p.PublicMessageId = @messageId
END;

-- PublicMessagesDataAccess.GetMessagesByUserIdAsync(string userId, int currentItemCount)
CREATE OR ALTER PROCEDURE sp_getPublicMessages_byUserId
	@userId nvarchar(450),
	@numberMessagesToSkip int
AS
BEGIN
	SELECT 
		p.PublicMessageId, 
		p.UserId, 
		a.UserName, 
		p.ChatGroupId, 
		c.ChatGroupName, 
		p.Text, 
		p.MessageDateTime, 
		p.ReplyMessageId, 
		p.PictureLink
	FROM PublicMessages p 
	JOIN AspNetUsers a
		ON p.UserId = a.Id
	JOIN ChatGroups c
		ON p.ChatGroupId = c.ChatGroupId
	WHERE p.UserId = @userId
	ORDER BY p.MessageDateTime DESC
	OFFSET @numberMessagesToSkip ROWS
	FETCH NEXT 50 ROWS ONLY
END;

-- PublicMessagesDataAccess.GetMessagesByGroupIdAsync(int groupId, int currentItemCount)
CREATE OR ALTER PROCEDURE sp_getPublicMessages_byGroupId
	@groupId int,
	@numberMessagesToSkip int
AS
BEGIN
	SELECT 
		p.PublicMessageId, 
		p.UserId, 
		a.UserName, 
		p.ChatGroupId, 
		c.ChatGroupName, 
		p.Text, 
		p.MessageDateTime, 
		p.ReplyMessageId, 
		p.PictureLink
	FROM PublicMessages p 
	JOIN AspNetUsers a
		ON p.UserId = a.Id
	JOIN ChatGroups c
		ON p.ChatGroupId = c.ChatGroupId
	WHERE p.ChatGroupId = @groupId
	ORDER BY p.MessageDateTime DESC
	OFFSET @numberMessagesToSkip ROWS
	FETCH NEXT 50 ROWS ONLY
END;