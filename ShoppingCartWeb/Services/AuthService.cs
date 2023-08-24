using Newtonsoft.Json.Linq;
using ShoppingCartWeb.Models;
using ShoppingCartWeb.Models.Dto;
using ShoppingCartWeb.Models.VM;
using ShoppingCartWeb.Services.IServices;
using ShoppingCartWeb.Utililty;

namespace ShoppingCartWeb.Services
{
    public class AuthService : BaseService, IAuthService
    {
        private readonly IHttpClientFactory _clientFactory;
        private string userURL;
        public AuthService(IHttpClientFactory clientFactory, IConfiguration configuration) : base(clientFactory)
        {
            this._clientFactory = clientFactory;
            userURL = configuration.GetValue<string>("ServiceUrls:ShoppingCartAPI");
        }

        public Task<T> LoginAsync<T>(LoginRequestDTO obj)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = obj,
                Url = userURL + "/api/UsersAuth/Login"
            });
        }

        public Task<T> RegisterAsync<T>(RegistrationDTO obj, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = obj,
                Url = userURL + "/api/UsersAuth/Register",
                Token = token
            }) ;
        }


        public Task<T> GetAllRegistrationAsync<T>(string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = userURL + "/api/UsersAuth/GetRegistration",
                Token = token
            });
        }

        public Task<T> UserRegisterAsync<T>(UserDTO obj, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = obj,
                Url = userURL + "/api/UsersAuth/UserRegister",
                Token = token
            });
        }

        public Task<T> GetAllCategoryAsync<T>(string token)
        {
            throw new NotImplementedException();
        }
    }
}
