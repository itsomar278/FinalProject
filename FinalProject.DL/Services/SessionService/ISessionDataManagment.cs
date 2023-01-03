namespace Domain.Services.SessionService
{
    public interface ISessionDataManagment
    {
        Task StoreUserInSession(string userEmail);
        UserSessionModel GetUserFromSession();
    }
}
