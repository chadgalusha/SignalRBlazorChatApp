using ChatApplicationModels;
using SignalRBlazorGroupsMessages.API.DataAccess;
using SignalRBlazorGroupsMessages.API.Helpers;
using SignalRBlazorGroupsMessages.API.Models;

namespace SignalRBlazorGroupsMessages.API.Services
{
    public class ChatGroupsService
    {
        private readonly IChatGroupsDataAccess _chatGroupsDataAccess;
        private readonly ISerilogger _serilogger;

        public ChatGroupsService(IChatGroupsDataAccess chatGroupsDataAccess, ISerilogger serilogger) 
        {
            _chatGroupsDataAccess = chatGroupsDataAccess ?? throw new Exception(nameof(chatGroupsDataAccess));
            _serilogger = serilogger ?? throw new Exception(nameof(serilogger));
        }

        public async Task<ApiResponse<List<ChatGroupsDto>>> GetViewListPublicChatGroupsAsync()
        {
            ApiResponse<List<ChatGroupsDto>> response = new();

            try
            {
                List<ChatGroupsView> viewList = await _chatGroupsDataAccess.GetViewListPublicChatGroupsAsync();
                List<ChatGroupsDto> dtoList = ViewListToDtoList(viewList);

                return ReturnApiResponse.Success(response, dtoList);
            }
            catch (Exception ex)
            {
                _serilogger.ChatGroupError("ChatGroupsService.ChatGroupsService", ex);

                response = ReturnApiResponse.Failure(response, "Error getting public chat groups.");
                response.Data = null;

                return response;
            }
        }

        public async Task<ApiResponse<List<ChatGroupsDto>>> GetViewListPrivateByUserId(Guid userId)
        {
            ApiResponse<List<ChatGroupsDto>> response = new();

            if (userId == new Guid())
            {
                response = ReturnApiResponse.Failure(response, "invalid userId");
                response.Data = null;
                return response;
            }

            try
            {
                List<ChatGroupsView> viewList = await _chatGroupsDataAccess.GetViewListPrivateByUserIdAsync(userId);
                List<ChatGroupsDto> dtoList = ViewListToDtoList(viewList);

                return ReturnApiResponse.Success(response, dtoList);
            }
            catch (Exception ex)
            {
                _serilogger.ChatGroupError("ChatGroupsService.GetViewListPrivateByUserId", ex);

                response = ReturnApiResponse.Failure(response, "Error getting private chat groups.");
                response.Data = null;

                return response;
            }
        }

        public async Task<ApiResponse<ChatGroupsDto>> GetChatGroupByIdAsync(int groupId)
        {
            ApiResponse<ChatGroupsDto> response = new();

            try
            {
                if (!GroupExists(groupId))
                {
                    response = ReturnApiResponse.Failure(response, "invalid groupId");
                    response.Data = null;
                    return response;
                }

                ChatGroupsView view = await _chatGroupsDataAccess.GetChatGroupByIdAsync(groupId);
                ChatGroupsDto dtoGroup = ViewToDto(view);

                return ReturnApiResponse.Success(response, dtoGroup);
            }
            catch (Exception ex)
            {
                _serilogger.ChatGroupError("ChatGroupsService.GetChatGroupByIdAsync", ex);

                response = ReturnApiResponse.Failure(response, "Error getting chat group.");
                response.Data = null;

                return response;
            }
        }

        public async Task<ApiResponse<ChatGroupsDto>> AddAsync(ChatGroupsDto dto)
        {
            ApiResponse<ChatGroupsDto> response = new();

            try
            {
                ChatGroups newChatGroup = DtoToModel(dto);
                bool isSuccess = await _chatGroupsDataAccess.AddAsync(newChatGroup);

                if (!isSuccess)
                {
                    response.Data = null;
                    return ReturnApiResponse.Failure(response, "Error saving new chat group.");
                }

                //ChatGroupsDto newDto = ViewToDto
                return response; // TODO fix this
            }
            catch (Exception ex)
            {
                _serilogger.ChatGroupError("ChatGroupsService.AddAsync", ex);

                response = ReturnApiResponse.Failure(response, "Error creating new chat group.");
                response.Data = null;

                return response;
            }
        }

        #region PRIVATE METHODS

        private bool GroupExists(int groupId)
        {
            return _chatGroupsDataAccess.GroupExists(groupId);
        }

        private List<ChatGroupsDto> ViewListToDtoList(List<ChatGroupsView> viewList)
        {
            List<ChatGroupsDto> dtoList = new();

            foreach (ChatGroupsView view in viewList)
            {
                ChatGroupsDto dto = new()
                {
                    ChatGroupId      = view.ChatGroupId,
                    ChatGroupName    = view.ChatGroupName,
                    GroupCreated     = view.GroupCreated,
                    GroupOwnerUserId = view.GroupOwnerUserId,
                    UserName         = view.UserName,
                    PrivateGroup     = view.PrivateGroup
                };
                dtoList.Add(dto);
            }
            return dtoList;
        }

        private ChatGroupsDto ViewToDto(ChatGroupsView view)
        {
            return new()
            {
                ChatGroupId      = view.ChatGroupId,
                ChatGroupName    = view.ChatGroupName,
                GroupCreated     = view.GroupCreated,
                GroupOwnerUserId = view.GroupOwnerUserId,
                UserName         = view.UserName,
                PrivateGroup     = view.PrivateGroup
            };
        }

        private ChatGroups DtoToModel(ChatGroupsDto dto)
        {
            return new()
            {
                ChatGroupId      = dto.ChatGroupId,
                ChatGroupName    = dto.ChatGroupName,
                GroupCreated     = dto.GroupCreated,
                GroupOwnerUserId = dto.GroupOwnerUserId,
                PrivateGroup     = dto.PrivateGroup
            };
        }

        #endregion
    }
}
