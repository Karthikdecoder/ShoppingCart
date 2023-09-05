using ShoppingCartWeb.Models.Dto;

namespace ShoppingCartWeb.Services.IServices
{
    public interface ICategoryService
    {
        Task<T> GetAllCategoryAsync<T>(string token);
        Task<T> GetCategoryAsync<T>(int categoryId, string token);
        Task<T> CreateCategoryAsync<T>(CategoryMasterDTO categoryMasterDTO, string token);
        Task<T> UpdateCategoryAsync<T>(CategoryMasterDTO categoryMasterDTO, string token);
        Task<T> EnableCategoryAsync<T>(int categoryId, string token);
        Task<T> RemoveCategoryAsync<T>(int categoryId, string token);
    }
}
