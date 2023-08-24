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
        public Task<T> CreateAsync<T>(RoleMasterDTO roleMasterDTO, string token)
        {
            throw new NotImplementedException();
        }

        public Task<T> DeleteAsync<T>(int roleId, string token)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetAllRolesAsync<T>(string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = roleUrl + "/api/RoleMaster/GetRoles",
                Token = token
            });
        }

        public Task<T> GetAsync<T>(int roleId, string token)
        {
            throw new NotImplementedException();
        }

        public Task<T> UpdateAsync<T>(RoleMasterDTO roleMasterDTO, string token)
        {
            throw new NotImplementedException();
        }
    }
}
