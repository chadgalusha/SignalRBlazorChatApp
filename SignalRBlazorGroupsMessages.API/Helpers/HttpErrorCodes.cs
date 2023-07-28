using SignalRBlazorGroupsMessages.API.Models;

namespace SignalRBlazorGroupsMessages.API.Helpers
{
    public static class HttpErrorCodes
    {
        public static int Get(string errorMessage)
        {
            return errorMessage switch
            {
                ErrorMessages.AddingItem         => StatusCodes.Status500InternalServerError,
                ErrorMessages.AddingUser         => StatusCodes.Status500InternalServerError,
                ErrorMessages.DeletingItem       => StatusCodes.Status500InternalServerError,
                ErrorMessages.DeletingMessages   => StatusCodes.Status500InternalServerError,
                ErrorMessages.DeletingUser       => StatusCodes.Status500InternalServerError,
                ErrorMessages.GroupNameTaken     => StatusCodes.Status400BadRequest,
                ErrorMessages.InvalidUserId      => StatusCodes.Status403Forbidden,
                ErrorMessages.ModifyingItem      => StatusCodes.Status500InternalServerError,
                ErrorMessages.NoModification     => StatusCodes.Status400BadRequest,
                ErrorMessages.RecordNotFound     => StatusCodes.Status404NotFound,
                ErrorMessages.ReplyMessages      => StatusCodes.Status500InternalServerError,
                ErrorMessages.RemovingUser       => StatusCodes.Status500InternalServerError,
                ErrorMessages.RetrievingItems    => StatusCodes.Status500InternalServerError,
                ErrorMessages.UserAlreadyInGroup => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status404NotFound
            };
        }
    }
}
