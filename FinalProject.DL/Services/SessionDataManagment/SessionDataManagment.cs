using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using WebApplication1.Models;
using WebApplication1.Models.Entites;
using WebApplication1.Services.SessionManagment;

namespace WebApplication1.Services.SessionDataManagment
{
    public class SessionDataManagment : ISessionDataManagment
    {
        private readonly IHttpContextAccessor _httpContextAccessor ;
        public SessionDataManagment(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public UserSessionModel GetUserFromSession()
        {
            string userJson = _httpContextAccessor.HttpContext.Session.GetString("MyUser");
            if (userJson != null)
            {
                return JsonConvert.DeserializeObject<UserSessionModel>(userJson);
            }
            else
            {
                return null;
            }
        }
        public void StoreUserInSession(UserSessionModel user)
        {
            string userJson = JsonConvert.SerializeObject(user);
            _httpContextAccessor.HttpContext.Session.SetString("MyUser", userJson);
        }
    }
}
