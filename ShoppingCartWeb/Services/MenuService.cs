using ShoppingCartWeb.Models;
using ShoppingCartWeb.Models.Dto;
using ShoppingCartWeb.Services.IServices;
using ShoppingCartWeb.Utililty;

namespace ShoppingCartWeb.Services
{
    public class MenuService : BaseService, IMenuService
    {
        private readonly IHttpClientFactory _clientFactory;
        private string menuURL;
        public MenuService(IHttpClientFactory clientFactory, IConfiguration configuration) : base(clientFactory)
        {
            this._clientFactory = clientFactory;

            menuURL = configuration.GetValue<string>("ServiceUrls:ShoppingCartAPI");
        }

        public Task<T> CreateMenuAsync<T>(MenuDTO MenuDTO, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = MenuDTO,
                Url = menuURL + "/api/Menu/CreateMenu",
                Token = token
            });
        }

        public Task<T> EnableMenuAsync<T>(int MenuId, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.PUT,
                Url = menuURL + "/api/Menu/EnableMenu?MenuId=" + MenuId,
                Token = token
            });
        }

        public Task<T> GetAllMenuAsync<T>(string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = menuURL + "/api/Menu/GetAllMenu",
                Token = token
            });
        }

        public Task<T> GetMenuAsync<T>(int MenuId, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = menuURL + "/api/Menu/GetMenu?MenuId=" + MenuId,
                Token = token
            });
        }

        public Task<T> RemoveMenuAsync<T>(int MenuId, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.DELETE,
                Url = menuURL + "/api/Menu/RemoveMenu?MenuId=" + MenuId,
                Token = token
            });
        }

        public Task<T> UpdateMenuAsync<T>(MenuDTO MenuDTO, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.PUT,
                Data = MenuDTO,
                Url = menuURL + "/api/Menu/UpdateMenu",
                Token = token
            });
        }
    }
}
