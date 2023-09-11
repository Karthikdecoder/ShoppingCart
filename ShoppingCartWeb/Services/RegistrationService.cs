﻿using Microsoft.EntityFrameworkCore.Internal;
using ShoppingCartWeb.Models;
using ShoppingCartWeb.Models.Dto;
using ShoppingCartWeb.Services.IServices;
using ShoppingCartWeb.Utililty;

namespace ShoppingCartWeb.Services
{
    public class RegistrationService : BaseService, IRegistrationService
    {
        private readonly IHttpClientFactory _clientFactory;
        private string registrationUrl;
        public RegistrationService(IHttpClientFactory clientFactory, IConfiguration configuration) : base(clientFactory)
        {
            this._clientFactory = clientFactory;

            registrationUrl = configuration.GetValue<string>("ServiceUrls:ShoppingCartAPI");
        }
        public Task<T> GetAllRegistrationAsync<T>(string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = registrationUrl + "/api/Registration/GetAllRegistration",
                Token = token
            });
        }

        public Task<T> GetRegistrationAsync<T>(int registrationId, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = registrationUrl + "/api/Registration/GetRegistration?registrationId=" + registrationId,
                Token = token
            });
        }

        public Task<T> CreateRegistrationAsync<T>(RegistrationDTO registrationDTO, string token)
        {
            return SendAsync<T>(new APIRequest() 
            {
                ApiType = SD.ApiType.POST,
                Data = registrationDTO,
                Url = registrationUrl + "/api/Registration/CreateRegistration",
                Token = token
            });
        }

        public Task<T> RemoveRegistrationAsync<T>(int registrationId, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.DELETE,
                Url = registrationUrl + "/api/Registration/RemoveRegistration?registrationId=" + registrationId,
                Token = token
            });
        }

        public Task<T> UpdateRegistrationAsync<T>(RegistrationDTO registrationDTO, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.PUT,
                Data = registrationDTO, 
                Url = registrationUrl + "/api/Registration/UpdateRegistration",
                Token = token
            });
        }

        public Task<T> GetAllDeletedRegistrationAsync<T>(string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = registrationUrl + "/api/Registration/GetAllDeletedRegistration",
                Token = token
            });
        }

        public Task<T> EnableRegistrationAsync<T>(int registrationId, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.PUT,
                Url = registrationUrl + "/api/Registration/EnableRegistration?registrationId=" + registrationId,
                Token = token
            });
        }
    }
}
