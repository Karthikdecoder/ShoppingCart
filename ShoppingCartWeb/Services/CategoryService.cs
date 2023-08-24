using ShoppingCartWeb.Models;
using ShoppingCartWeb.Models.Dto;
using ShoppingCartWeb.Services.IServices;
using ShoppingCartWeb.Utililty;

namespace ShoppingCartWeb.Services
{
    public class CategoryService : BaseService, ICategoryService
    {
        private readonly IHttpClientFactory _clientFactory;
        private string categoryURL;
        public CategoryService(IHttpClientFactory clientFactory, IConfiguration configuration) : base(clientFactory)
        {
            this._clientFactory = clientFactory;

            categoryURL = configuration.GetValue<string>("ServiceUrls:ShoppingCartAPI");
        }
        public Task<T> CreateAsync<T>(CategoryMasterDTO categoryMasterDTO, string token)
        {
            throw new NotImplementedException();
        }

        public Task<T> DeleteAsync<T>(int roleId, string token)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetAllAsync<T>(string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = categoryURL + "/api/CategoryMaster/GetCategory",
                Token = token
            });
        }

        public Task<T> GetAsync<T>(int roleId, string token)
        {
            throw new NotImplementedException();
        }

        public Task<T> UpdateAsync<T>(CategoryMasterDTO categoryMasterDTO, string token)
        {
            throw new NotImplementedException();
        }
    }
}
