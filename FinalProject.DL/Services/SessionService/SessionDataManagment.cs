﻿using AutoMapper;
using DataAcess.UnitOfWorks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Domain.Services.SessionService
{
    public class SessionDataManagment : ISessionDataManagment
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public SessionDataManagment(IHttpContextAccessor httpContextAccessor, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public UserSessionModel GetUserFromSession()
        {
            string userJson = _httpContextAccessor.HttpContext.Session.GetString("MyUser");

            if (userJson != null)
            {
                return JsonConvert.DeserializeObject<UserSessionModel>(userJson);
            }

            return null;

        }
        public async Task StoreUserInSession(string userEmail)
        {
            var userResult = await _unitOfWork.Users.FindByEmailAsync(userEmail);
            var userInSession = _mapper.Map<UserSessionModel>(userResult);
            string userJson = JsonConvert.SerializeObject(userInSession);
            _httpContextAccessor.HttpContext.Session.SetString("MyUser", userJson);
        }

    }
}
