using ShoppingCartWeb.Models;
using ShoppingCartWeb.Models.Dto;
using ShoppingCartWeb.Services.IServices;
using ShoppingCartWeb.Utililty;

namespace ShoppingCartWeb.Services
{
    public class RoleService : BaseService, IRoleService
    {
        private readonly IHttpClientFactory _clientFactory;
        private string roleUrl;
        public RoleService(IHttpClientFactory clientFactory, IConfiguration configuration) : base(clientFactory) 
        {
            this._clientFactory = clientFactory;

            roleUrl = configuration.GetValue<string>("ServiceUrls:ShoppingCartAPI");
        }

        public Task<T> GetAllRoleAsync<T>(string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = roleUrl + "/api/RoleMaster/GetRoles",
                Token = token
            });
        }

        public Task<T> GetRoleAsync<T>(int roleId, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = roleUrl + "/api/RoleMaster/GetRole?roleId=" + roleId,
                Token = token
            });
        }

        public Task<T> CreateRoleAsync<T>(RoleMasterDTO roleMasterDTO, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = roleMasterDTO,
                Url = roleUrl + "/api/RoleMaster/CreateRole",
                Token = token
            });
        }

        public Task<T> RemoveRoleAsync<T>(int roleId, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.DELETE,
                Url = roleUrl + "/api/RoleMaster/RemoveRole?roleId=" + roleId,
                Token = token
            });
        }

        public Task<T> UpdateRoleAsync<T>(RoleMasterDTO roleMasterDTO, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.PUT,
                Data = roleMasterDTO, 
                Url = roleUrl + "/api/RoleMaster/UpdateRole",
                Token = token
            });
        }

        public Task<T> EnableRoleAsync<T>(int roleId, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.PUT,
                Url = roleUrl + "/api/RoleMaster/EnableRole?roleId=" + roleId,
                Token = token
            });
        }
    }
}
