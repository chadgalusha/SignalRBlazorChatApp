/* SQL file to create non-ASP.NET Identity tables needed */

USE ChatApplication

DROP TABLE IF EXISTS PrivateGroupMessages;
DROP TABLE IF EXISTS PublicMessages;
DROP TABLE IF EXISTS UserFriends;
DROP TABLE IF EXISTS FriendRequests;
DROP TABLE IF EXISTS PrivateUserMessages;
DROP TABLE IF EXISTS BlockedPrivateChatGroups;
DROP TABLE IF EXISTS BlockedUsers;
DROP TABLE IF EXISTS PrivateGroupInvitations;
DROP TABLE IF EXISTS PrivateGroupRequests;
DROP TABLE IF EXISTS ChangeGroupOwnerRequests;
DROP TABLE IF EXISTS PrivateGroupMembers;
DROP TABLE IF EXISTS PrivateChatGroups;
DROP TABLE IF EXISTS PublicChatGroups;


CREATE TABLE PublicChatGroups (
	ChatGroupId int NOT NULL IDENTITY(1,1),
	ChatGroupName varchar(200) NOT NULL,
	GroupCreated DateTime,
	GroupOwnerUserId nvarchar(450),
	PRIMARY KEY (ChatGroupId),
	CONSTRAINT FK_PublicChatGroups_GroupOwnerUserId FOREIGN KEY (GroupOwnerUserId) REFERENCES AspNetUsers(Id),
	CONSTRAINT PublicChatGroupName_Unique UNIQUE (ChatGroupName)
);

CREATE TABLE PrivateChatGroups (
	ChatGroupId int NOT NULL IDENTITY(1,1),
	ChatGroupName varchar(200) NOT NULL,
	GroupCreated DateTime,
	GroupOwnerUserId nvarchar(450),
	PRIMARY KEY (ChatGroupId),
	CONSTRAINT FK_PrivateChatGroups_GroupOwnerUserId FOREIGN KEY (GroupOwnerUserId) REFERENCES AspNetUsers(Id),
	CONSTRAINT PrivateChatGroupName_Unique UNIQUE (ChatGroupName)
);

CREATE TABLE PrivateGroupMembers (
	PrivateGroupMemberId int NOT NULL IDENTITY(1,1),
	PrivateChatGroupId int NOT NULL,
	UserId nvarchar(450) NOT NULL,
	PRIMARY KEY (PrivateGroupMemberId),
	CONSTRAINT FK_PrivateGroupMembers_PrivateChatGroupId FOREIGN KEY (PrivateChatGroupId) REFERENCES PrivateChatGroups(ChatGroupId),
	CONSTRAINT FK_PrivateGroupMembers_UserId FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id)
);

CREATE TABLE ChangeGroupOwnerRequests (
	Id int NOT NULL IDENTITY(1,1),
	PublicChatGroupId int,
	PrivateChatGroupId int,
	IsPrivate bit NOT NULL,
	CurrentOwnerUserid nvarchar(450) NOT NULL,
	NewOwnerUserId nvarchar(450) NOT NULL,
	RequestText varchar(200),
	RequestSeen bit,
	ChangeOwnerDateTime DateTime,
	PRIMARY KEY (Id),
	CONSTRAINT FK_ChangeGroupOwnerRequests_PublicChatGroupId FOREIGN KEY (PublicChatGroupId) REFERENCES PublicChatGroups (ChatGroupId),
	CONSTRAINT FK_ChangeGroupOwnerRequests_PrivateChatGroupId FOREIGN KEY (PrivateChatGroupId) REFERENCES PrivateChatGroups (ChatGroupId),
	CONSTRAINT FK_ChangeGroupOwnerRequests_CurrentOwnerUserid FOREIGN KEY (CurrentOwnerUserid) REFERENCES AspNetUsers(Id),
	CONSTRAINT FK_ChangeGroupOwnerRequests_NewOwnerUserId FOREIGN KEY (NewOwnerUserId) REFERENCES AspNetUsers(Id)
);

CREATE TABLE PrivateGroupRequests (
	PrivateGroupRequestId int NOT NULL IDENTITY(1,1),
	ChatGroupId int NOT NULL,
	RequestUserId nvarchar(450) NOT NULL,
	RequestText varchar(200),
	GroupRequestDateTime DateTime,
	PRIMARY KEY (PrivateGroupRequestId),
	CONSTRAINT FK_PrivateGroupRequests_ChatGroupId FOREIGN KEY (ChatGroupId) REFERENCES PrivateChatGroups(ChatGroupId),
	CONSTRAINT FK_PrivateGroupRequests_RequestUserId FOREIGN KEY (RequestUserId) REFERENCES AspNetUsers(Id)
);

CREATE TABLE PrivateGroupInvitations (
	PrivateGroupInvitationId int NOT NULL IDENTITY(1,1),
	ChatGroupId int NOT NULL,
	GroupOwnerUserId nvarchar(450),
	InvitedUserId nvarchar(450) NOT NULL,
	InvitationText varchar(200),
	InvitationSeen bit,
	InvitationDateTime DateTime,
	PRIMARY KEY (PrivateGroupInvitationId),
	CONSTRAINT FK_PrivateGroupInvitations_ChatGroupId FOREIGN KEY (ChatGroupId) REFERENCES PrivateChatGroups(ChatGroupId),
	CONSTRAINT FK_PrivateGroupInvitations_GroupOwnerUserId FOREIGN KEY (GroupOwnerUserId) REFERENCES AspNetUsers(Id),
	CONSTRAINT FK_PrivateGroupInvitations_InvitedUserId FOREIGN KEY (InvitedUserId) REFERENCES AspNetUsers(Id)
);

CREATE TABLE BlockedUsers (
	Id int NOT NULL IDENTITY(1,1),
	UserId nvarchar(450) NOT NULL,
	BlockedUserId nvarchar(450) NOT NULL,
	PRIMARY KEY (Id),
	CONSTRAINT FK_BlockedUsers_UserId FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id),
	CONSTRAINT FK_BlockedUsers_BlockedUserId FOREIGN KEY (BlockedUserId) REFERENCES AspNetUsers(Id)
);

CREATE TABLE BlockedPrivateChatGroups (
	Id int NOT NULL IDENTITY(1,1),
	UserId nvarchar(450) NOT NULL,
	BlockedChatGroupId int,
	PRIMARY KEY (Id),
	CONSTRAINT FK_BlockedPrivateChatGroups_UserId FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id),
	CONSTRAINT FK_BlockedPrivateChatGroups_BlockedChatGroupId FOREIGN KEY (BlockedChatGroupId) REFERENCES PrivateChatGroups(ChatGroupId)
);

CREATE TABLE PrivateUserMessages (
	PrivateMessageId int NOT NULL IDENTITY(1,1),
	FromUserId nvarchar(450) NOT NULL,
	ToUserId nvarchar(450) NOT NULL,
	MessageText varchar(200),
	MessageSeen bit,
	PrivateMessageDateTime DateTime,
	PRIMARY KEY (PrivateMessageId),
	CONSTRAINT FK_PrivateUserMessages_FromUserId FOREIGN KEY (FromUserId) REFERENCES AspNetUsers(Id),
	CONSTRAINT FK_PrivateUserMessages_ToUserId FOREIGN KEY (ToUserId) REFERENCES AspNetUsers(Id)
);

CREATE TABLE FriendRequests (
	FriendRequestId int NOT NULL IDENTITY(1,1),
	RequestUserId nvarchar(450) NOT NULL,
	RecipientUserId nvarchar(450) NOT NULL,
	RequestSeen bit,
	FriendRequestDateTime DateTime,
	PRIMARY KEY (FriendRequestId),
	CONSTRAINT FK_FriendRequests_RequestUserId FOREIGN KEY (RequestUserId) REFERENCES AspNetUsers(Id),
	CONSTRAINT FK_FriendRequests_RecipientUserId FOREIGN KEY (RecipientUserId) REFERENCES AspNetUsers(Id)
);

CREATE TABLE UserFriends (
	UserFriendId int NOT NULL IDENTITY(1,1),
	UserId nvarchar(450) NOT NULL,
	FriendUserid nvarchar(450) NOT NULL,
	FriendsSinceDateTime DateTime,
	PRIMARY KEY (UserFriendId),
	CONSTRAINT FK_UserFriends_UserId FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id),
	CONSTRAINT FK_UserFriends_FriendUserid FOREIGN KEY (FriendUserid) REFERENCES AspNetUsers(Id)
);

CREATE TABLE PublicGroupMessages (
	PublicMessageId uniqueidentifier NOT NULL,
	UserId nvarchar(450) NOT NULL,
	ChatGroupId int NOT NULL,
	Text varchar(max),
	MessageDateTime DateTime,
	ReplyMessageId uniqueidentifier,
	PictureLink varchar(max),
	PRIMARY KEY (PublicMessageId),
	CONSTRAINT FK_PublicGroupMessages_UserId FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id),
	CONSTRAINT FK_PublicGroupMessages_ChatGroupId FOREIGN KEY (ChatGroupId) REFERENCES PublicChatGroups(ChatGroupId)
);

CREATE TABLE PrivateGroupMessages (
	PrivateMessageId uniqueidentifier NOT NULL,
	UserId nvarchar(450) NOT NULL,
	ChatGroupId int NOT NULL,
	Text varchar(max),
	MessageDateTime DateTime,
	ReplyMessageId uniqueidentifier,
	PictureLink varchar(max),
	PRIMARY KEY (PrivateMessageId),
	CONSTRAINT FK_PrivateGroupMessages_UserId FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id),
	CONSTRAINT FK_PrivateMessages_ChatGroupId FOREIGN KEY (ChatGroupId) REFERENCES PrivateChatGroups(ChatGroupId)
);

/*
-- Sample data to input for new tables
INSERT INTO PublicChatGroups(ChatGroupName, GroupCreated, GroupOwnerUserId)
VALUES('TestPublicTable1', SYSDATETIME(), '93eeda54-e362-49b7-8fd0-ab516b7f8071');

INSERT INTO PublicChatGroups(ChatGroupName, GroupCreated, GroupOwnerUserId)
VALUES('TestPublicTable2', SYSDATETIME(), '93eeda54-e362-49b7-8fd0-ab516b7f8071');

INSERT INTO PrivateChatGroups(ChatGroupName, GroupCreated, GroupOwnerUserId)
VALUES('TestPrivateTable1', SYSDATETIME(), '93eeda54-e362-49b7-8fd0-ab516b7f8071');

INSERT INTO PrivateChatGroups(ChatGroupName, GroupCreated, GroupOwnerUserId)
VALUES('TestPrivateTable2', SYSDATETIME(), 'e08b0077-3c15-477e-84bb-bf9d41196455');
*/

/*
-- Sample data to input for PublicGroupMessages. This assumes the ChatGroupId is valid. may need to adjust
INSERT INTO PublicGroupMessages(PublicMessageId, UserId, ChatGroupId, Text, MessageDateTime)
VALUES(NEWID(), '93eeda54-e362-49b7-8fd0-ab516b7f8071', 1, 'Test message from Admin', SYSDATETIME());

INSERT INTO PublicGroupMessages(PublicMessageId, UserId, ChatGroupId, Text, MessageDateTime)
VALUES(NEWID(), 'e08b0077-3c15-477e-84bb-bf9d41196455', 1, 'Test message from TestUser1', SYSDATETIME());

INSERT INTO PublicGroupMessages(PublicMessageId, UserId, ChatGroupId, Text, MessageDateTime)
VALUES(NEWID(), '93eeda54-e362-49b7-8fd0-ab516b7f8071', 2, 'Test message from Admin', SYSDATETIME());

INSERT INTO PublicGroupMessages(PublicMessageId, UserId, ChatGroupId, Text, MessageDateTime)
VALUES(NEWID(), 'e08b0077-3c15-477e-84bb-bf9d41196455', 2, 'Test message from TestUser1', SYSDATETIME());
*/
/*
-- Sample data for PrivateGroupMembers table. Dependent on Private chat group tables already created. check before running.
INSERT INTO PrivateGroupMembers(PrivateChatGroupId, UserId)
VALUES(1, '93eeda54-e362-49b7-8fd0-ab516b7f8071');

INSERT INTO PrivateGroupMembers(PrivateChatGroupId, UserId)
VALUES(1, 'e08b0077-3c15-477e-84bb-bf9d41196455');

INSERT INTO PrivateGroupMembers(PrivateChatGroupId, UserId)
VALUES(2, '93eeda54-e362-49b7-8fd0-ab516b7f8071');

INSERT INTO PrivateGroupMembers(PrivateChatGroupId, UserId)
VALUES(2, 'e08b0077-3c15-477e-84bb-bf9d41196455');
*/

/*
-- Sample data for PrivateGroupMessages. Dependent on private chat groups already existing
INSERT INTO PrivateGroupMessages(PrivateMessageId, UserId, ChatGroupId, Text, MessageDateTime)
VALUES(NEWID(), '93eeda54-e362-49b7-8fd0-ab516b7f8071', 1, 'Test Text', SYSDATETIME());

INSERT INTO PrivateGroupMessages(PrivateMessageId, UserId, ChatGroupId, Text, MessageDateTime)
VALUES(NEWID(), 'e08b0077-3c15-477e-84bb-bf9d41196455', 1, 'Test Text', SYSDATETIME());

INSERT INTO PrivateGroupMessages(PrivateMessageId, UserId, ChatGroupId, Text, MessageDateTime)
VALUES(NEWID(), '93eeda54-e362-49b7-8fd0-ab516b7f8071', 2, 'Test Text', SYSDATETIME());

INSERT INTO PrivateGroupMessages(PrivateMessageId, UserId, ChatGroupId, Text, MessageDateTime)
VALUES(NEWID(), 'e08b0077-3c15-477e-84bb-bf9d41196455', 2, 'Test Text', SYSDATETIME());
*/
