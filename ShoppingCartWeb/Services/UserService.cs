using Newtonsoft.Json.Linq;
using ShoppingCartWeb.Models;
using ShoppingCartWeb.Models.Dto;
using ShoppingCartWeb.Models.VM;
using ShoppingCartWeb.Services.IServices;
using ShoppingCartWeb.Utililty;

namespace ShoppingCartWeb.Services
{
    public class UserService : BaseService, IUserService
    {
        private readonly IHttpClientFactory _clientFactory;
        private string userURL;
        public UserService(IHttpClientFactory clientFactory, IConfiguration configuration) : base(clientFactory)
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
                Url = userURL + "/api/User/Login"
            });
        }

        public Task<T> RegisterAsync<T>(UserDTO obj, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = obj,
                Url = userURL + "/api/User/Register",
                Token = token
            }) ;
        }

        public Task<T> GetAllUserAsync<T>(string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = userURL + "/api/User/GetAllUser",
                Token = token
            });
        }

        public Task<T> GetUserAsync<T>(int userId, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = userURL + "/api/User/GetUser?userId=" + userId,
                Token = token
            });
        }

        public Task<T> UpdateUserAsync<T>(UserDTO userDTO, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.PUT,
                Data = userDTO,
                Url = userURL + "/api/User/UpdateUser", 
                Token = token
            });
        }

        public Task<T> RemoveUserAsync<T>(int userId, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.DELETE,
                Url = userURL + "/api/User/RemoveUser?userId=" + userId,
                Token = token
            });
        }
    }
}
