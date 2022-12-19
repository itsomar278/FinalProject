using Microsoft.AspNetCore.Http.Features;
using WebApplication1.Models;
using WebApplication1.Models.Entites;

namespace WebApplication1.Services.SessionManagment
{
    public interface ISessionDataManagment
    {
        Task StoreUserInSession(string userEmail);
        UserSessionModel GetUserFromSession();
    }
}
