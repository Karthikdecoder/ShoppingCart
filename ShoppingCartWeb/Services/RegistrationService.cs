using Microsoft.EntityFrameworkCore.Internal;
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

        public Task<T> CreateAsync<T>(RegistrationDTO registrationDTO, string token)
        {
            throw new NotImplementedException();
        }

        public Task<T> DeleteAsync<T>(int roleId, string token)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetAllAsync<T>(string token)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetAsync<T>(int roleId, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = registrationUrl + "/api/UsersAuth/GetRegistration",
                Token = token
            });
        }

        public Task<T> UpdateAsync<T>(RegistrationDTO registrationDTO, string token)
        {
            throw new NotImplementedException();
        }
    }
}
