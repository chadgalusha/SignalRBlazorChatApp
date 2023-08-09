# SignalRBlazorChatApp

The purpose of this practice project is to develop an online chat application somewhat like Discord, but at a much smaller scale (as it is just me).
I am using Visual Studio as my IDE. For the backend system, I am building and using REST APIs with .NET Core. The front end will be Blazor Server. I
have also included a separate project for unit testing, another for a class library, and another for a Json Web Token generator for testing. If you 
do not see anything on the Master branch, check Development and any sub-branches. For data storage I am using Entity Framework Core tied to SQL Server.

Live testing is all being done in a local environment (localhost:port) with Postman. 

List of projects within the solution:
1. ChatApplicationModels - class library for shared use of models within the other projects
2. JWTGenerator - generate JWT for testing within Postman
3. SignalRBlazorChatApp - Blazor Server front end application. Will use MudBlazor nuget package for assistance with front-end design and implementation
4. SignalRBlazorGroupsMessages.API - REST API for endpoints with public/private chat groups and messages
5. SignalRBlazorRequestsInvitations.API - REST API for endpoints with requests to join groups, become friends with other users, private chats, etc.
6. SignalRBlazorUnitTests - unit testing for front end and API projects

Notes:
- SupportFiles folder includes SQL files for intended use with SQL Server. Included is files for building the tables
  and dummy data, along with stored procedures.
