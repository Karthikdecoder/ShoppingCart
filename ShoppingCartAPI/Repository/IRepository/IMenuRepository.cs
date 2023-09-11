using ShoppingCartAPI.Models;

namespace ShoppingCartAPI.Repository.IRepository
{
    public interface IMenuRepository : IRepository<Menu>
    {
        Task<Menu> UpdateAsync(Menu entity);
    }
}
