using ShoppingCartAPI.Models;

namespace ShoppingCartAPI.Repository.IRepository
{
    public interface IMenuRoleMappingRepository : IRepository<MenuRoleMapping>
    {
        Task<MenuRoleMapping> UpdateAsync(MenuRoleMapping entity);
    }
}
