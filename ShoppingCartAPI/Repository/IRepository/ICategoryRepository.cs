using ShoppingCartAPI.Models;

namespace ShoppingCartAPI.Repository.IRepository
{
    public interface ICategoryRepository : IRepository<CategoryMaster>
    {
        Task<CategoryMaster> UpdateAsync(CategoryMaster entity);
    }
}
