-- ################################################################################# --
-- Stored Procedures for use in Data Access layer in SignalRBlazorGroupsMessages.API --
-- ################################################################################# --

-- 
CREATE OR ALTER PROCEDURE sp_getPrivateChatGroupsForUser
	@userId nvarchar(450)
AS
BEGIN
	SELECT
		c.ChatGroupId,
		c.ChatGroupName,
		c.GroupCreated,
		c.GroupOwnerUserId,
		a.UserName,
		c.PrivateGroup
	FROM ChatGroups c, AspNetUsers a, PrivateGroupMembers p
	WHERE c.ChatGroupId = p.PrivateChatGroupId
	AND p.UserId = @userId
	AND c.GroupOwnerUserId = a.Id
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

-- ChatGroupsDataAccess.GetPublicChatGroupsAsync()
CREATE OR ALTER PROCEDURE sp_getPublicChatGroups
AS
BEGIN
	SELECT 
		c.ChatGroupId,
		c.ChatGroupName,
		c.GroupCreated,
		c.GroupOwnerUserId,
		a.UserName,
		c.PrivateGroup
	FROM ChatGroups c
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
		a.UserName,
		c.PrivateGroup
	FROM ChatGroups c
	JOIN AspNetUsers a
		ON c.GroupOwnerUserId = a.Id
	WHERE c.ChatGroupId = @groupId
END;

-- added unique constraint to ChatGroups. each chat group will be required to have a unique name
ALTER TABLE ChatGroups ADD CONSTRAINT chatgroupname_unique UNIQUE (ChatGroupName);