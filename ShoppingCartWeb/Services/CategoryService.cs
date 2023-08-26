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
        public Task<T> CreateCategoryAsync<T>(CategoryMasterDTO categoryMasterDTO, string token)
        {
            throw new NotImplementedException();
        }

        public Task<T> RemoveCategoryAsync<T>(int categoryId, string token)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetAllCategoryAsync<T>(string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = categoryURL + "/api/CategoryMaster/GetAllCategory",
                Token = token
            });
        }

        public Task<T> GetCategoryAsync<T>(int categoryId, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = categoryURL + "/api/CategoryMaster/GetCategory?categoryId?=" + categoryId,
                Token = token
            });
        }

        public Task<T> UpdateCategoryAsync<T>(CategoryMasterDTO categoryMasterDTO, string token)
        {
            throw new NotImplementedException();
        }
    }
}
