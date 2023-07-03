using SignalRBlazorGroupsMessages.API.Models;

namespace SignalRBlazorGroupsMessages.API.Helpers
{
    public static class ReturnApiResponse
    {
        public static ApiResponse<T> Success<T>(ApiResponse<T> response, T dtoData)
        {
            response.Success = true;
            response.Message = "ok";
            response.Data = dtoData;

            return response;
        }

        public static ApiResponse<T> Failure<T>(ApiResponse<T> response, string errorMessage)
        {
            response.Success = false;
            response.Message = errorMessage;

            return response;
        }
    }
}
