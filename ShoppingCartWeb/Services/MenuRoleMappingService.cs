using ShoppingCartWeb.Models;
using ShoppingCartWeb.Models.Dto;
using ShoppingCartWeb.Services.IServices;
using ShoppingCartWeb.Utililty;

namespace ShoppingCartWeb.Services
{
    public class MenuRoleMappingService : BaseService, IMenuRoleMappingService
    {
        private readonly IHttpClientFactory _clientFactory;
        private string menuRoleMappingURL;
        public MenuRoleMappingService(IHttpClientFactory clientFactory, IConfiguration configuration) : base(clientFactory)
        {
            this._clientFactory = clientFactory;

            menuRoleMappingURL = configuration.GetValue<string>("ServiceUrls:ShoppingCartAPI");
        }

        public Task<T> CreateMenuRoleMappingAsync<T>(MenuRoleMappingDTO MenuRoleMappingDTO, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = MenuRoleMappingDTO,
                Url = menuRoleMappingURL + "/api/MenuRoleMapping/CreateMenuRoleMapping",
                Token = token
            });
        }

        public Task<T> EnableMenuRoleMappingAsync<T>(int MenuRoleMappingId, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.PUT,
                Url = menuRoleMappingURL + "/api/MenuRoleMapping/EnableMenuRoleMapping?MenuRoleMappingId=" + MenuRoleMappingId,
                Token = token
            });
        }

        public Task<T> GetAllMenuRoleMappingAsync<T>(string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = menuRoleMappingURL + "/api/MenuRoleMapping/GetAllMenuRoleMapping",
                Token = token
            });
        }

        public Task<T> GetMenuRoleMappingAsync<T>(int MenuRoleMappingId, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = menuRoleMappingURL + "/api/MenuRoleMapping/GetMenuRoleMapping?MenuRoleMappingId=" + MenuRoleMappingId,
                Token = token
            });
        }

        public Task<T> RemoveMenuRoleMappingAsync<T>(int MenuRoleMappingId, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.DELETE,
                Url = menuRoleMappingURL + "/api/MenuRoleMapping/RemoveMenuRoleMapping?MenuRoleMappingId=" + MenuRoleMappingId,
                Token = token
            });
        }

        public Task<T> UpdateMenuRoleMappingAsync<T>(MenuRoleMappingDTO MenuRoleMappingDTO, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.PUT,
                Data = MenuRoleMappingDTO,
                Url = menuRoleMappingURL + "/api/MenuRoleMapping/UpdateMenuRoleMapping",
                Token = token
            });
        }
    }
}
