-- ################################################################################# --
-- Stored Procedures for use in Data Access layer in SignalRBlazorGroupsMessages.API --
-- ################################################################################# --

-- Get List of chat groups that user has access to.
CREATE OR ALTER PROCEDURE sp_getPrivateChatGroupsForUser
	@userId nvarchar(450)
AS
BEGIN
	SELECT
		c.ChatGroupId,
		c.ChatGroupName,
		c.GroupCreated,
		c.GroupOwnerUserId,
		a.UserName
	FROM PrivateChatGroups c, AspNetUsers a, PrivateGroupMembers p
	WHERE c.ChatGroupId = p.PrivateChatGroupId
	AND p.UserId = @userId
	AND c.GroupOwnerUserId = a.Id
END

-- Get a single private chat group by its groupId
CREATE OR ALTER PROCEDURE sp_getPrivateChatGroupByGroupId
	@groupId int
AS
BEGIN
	SELECT
		c.ChatGroupId,
		c.ChatGroupName,
		c.GroupCreated,
		c.GroupOwnerUserId,
		a.UserName
	FROM PrivateChatGroups c, AspNetUsers a
	WHERE c.GroupOwnerUserId = a.Id
	AND c.ChatGroupId = @groupId
END

-- PublicMessagesDataAccess.GetPublicMessageByIdAsync(string messageId)
CREATE OR ALTER PROCEDURE sp_getPublicMessage_byMessageId
	@messageId uniqueidentifier
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
	FROM PublicGroupMessages p 
	JOIN AspNetUsers a
		ON p.UserId = a.Id
	JOIN PublicChatGroups c
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
	FROM PublicGroupMessages p 
	JOIN AspNetUsers a
		ON p.UserId = a.Id
	JOIN PublicChatGroups c
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
	FROM PublicGroupMessages p 
	JOIN AspNetUsers a
		ON p.UserId = a.Id
	JOIN PublicChatGroups c
		ON p.ChatGroupId = c.ChatGroupId
	WHERE p.ChatGroupId = @groupId
	ORDER BY p.MessageDateTime DESC
	OFFSET @numberMessagesToSkip ROWS
	FETCH NEXT 50 ROWS ONLY
END;

-- ChatGroupsDataAccess.GetPublicChatGroupsAsync()
CREATE OR ALTER PROCEDURE sp_getPublicChatGroups
AS
BEGIN
	SELECT 
		c.ChatGroupId,
		c.ChatGroupName,
		c.GroupCreated,
		c.GroupOwnerUserId,
		a.UserName
	FROM PublicChatGroups c
	JOIN AspNetUsers a
		ON c.GroupOwnerUserId = a.Id
END;

-- ChatGroupsDataAccess.GetChatGroupByIdAsync(int groupId)
CREATE OR ALTER PROCEDURE sp_getChatGroup_byGroupId
	@groupId int
AS
BEGIN
	SELECT 
		c.ChatGroupId,
		c.ChatGroupName,
		c.GroupCreated,
		c.GroupOwnerUserId,
		a.UserName
	FROM PublicChatGroups c
	JOIN AspNetUsers a
		ON c.GroupOwnerUserId = a.Id
	WHERE c.ChatGroupId = @groupId
END;

-- PrivateMessagesDataAccess.GetDtoListByGroupId(int groupId, int numberMessagesToSkip)
CREATE OR ALTER PROCEDURE sp_getPrivateGroupMessages_byGroupId
	@groupId int,
	@numberMessagesToSkip int
AS
BEGIN
	SELECT
		p.PrivateMessageId,
		p.UserId,
		a.UserName,
		p.ChatGroupId,
		c.ChatGroupName,
		p.Text,
		p.MessageDateTime,
		p.ReplyMessageId,
		p.PictureLink
	FROM PrivateGroupMessages p
	JOIN AspNetUsers a
		ON p.UserId = a.Id
	JOIN PrivateChatGroups c
		ON p.ChatGroupId = c.ChatGroupId
	WHERE p.ChatGroupId = @groupId
	ORDER BY p.MessageDateTime DESC
	OFFSET @numberMessagesToSkip ROWS
	FETCH NEXT 50 ROWS ONLY
END

-- PrivateMessagesDataAccess.GetDtoListByUserIdAsync(string userId, int numberMessagesToSkip)
CREATE OR ALTER PROCEDURE sp_getPrivateGroupMessages_byUserId
	@userId nvarchar(450),
	@numberMessagesToSkip int
AS
BEGIN
	SELECT
		p.PrivateMessageId,
		p.UserId,
		a.UserName,
		p.ChatGroupId,
		c.ChatGroupName,
		p.Text,
		p.MessageDateTime,
		p.ReplyMessageId,
		p.PictureLink
	FROM PrivateGroupMessages p
	JOIN AspNetUsers a
		ON p.UserId = a.Id
	JOIN PrivateChatGroups c
		ON p.ChatGroupId = c.ChatGroupId
	WHERE p.UserId = @userId
	ORDER BY p.MessageDateTime DESC
	OFFSET @numberMessagesToSkip ROWS
	FETCH NEXT 50 ROWS ONLY
END

-- PrivateMessagesDataAccess.GetDtoByMessageIdAsync(Guid messageId)
CREATE OR ALTER PROCEDURE sp_getPrivateGroupMessage_byMessageId
	@messageId uniqueidentifier
AS
BEGIN
	SELECT
		p.PrivateMessageId,
		p.UserId,
		a.UserName,
		p.ChatGroupId,
		c.ChatGroupName,
		p.Text,
		p.MessageDateTime,
		p.ReplyMessageId,
		p.PictureLink
	FROM PrivateGroupMessages p
	JOIN AspNetUsers a
		ON p.UserId = a.Id
	JOIN PrivateChatGroups c
		ON p.ChatGroupId = c.ChatGroupId
	WHERE p.PrivateMessageId = @messageId
END